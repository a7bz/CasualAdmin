namespace CasualAdmin.Infrastructure.FileStorage;
using System.Collections.Concurrent;

/// <summary>
/// 分片上传会话信息
/// </summary>
public class MultipartUploadSession
{
    /// <summary>
    /// 文件唯一标识
    /// </summary>
    public string? FileId { get; set; }

    /// <summary>
    /// 文件名
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 文件内容类型
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// 存储路径
    /// </summary>
    public string? FolderPath { get; set; }

    /// <summary>
    /// 分片总数
    /// </summary>
    public int TotalParts { get; set; }

    /// <summary>
    /// 文件总大小
    /// </summary>
    public long TotalSize { get; set; }

    /// <summary>
    /// 临时目录路径
    /// </summary>
    public string? TempDirectoryPath { get; set; }

    /// <summary>
    /// 已上传的分片索引集合
    /// </summary>
    public HashSet<int> UploadedParts { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public MultipartUploadSession()
    {
        UploadedParts = [];
        CreatedAt = DateTime.Now;
    }
}

/// <summary>
/// 本地文件存储服务
/// </summary>
public class LocalFileStorageService : AbstractFileStorageService
{
    private readonly ConcurrentDictionary<string, MultipartUploadSession> _uploadSessions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="config">文件存储配置</param>
    public LocalFileStorageService(FileStorageConfig config) : base(config)
    {
        // 验证本地存储路径
        if (string.IsNullOrEmpty(config.LocalPath))
        {
            throw new ArgumentNullException(nameof(config.LocalPath), "本地存储路径不能为空");
        }

        // 确保存储目录存在
        if (!Directory.Exists(config.LocalPath))
        {
            Directory.CreateDirectory(config.LocalPath);
        }

        _uploadSessions = new ConcurrentDictionary<string, MultipartUploadSession>();
    }

    /// <inheritdoc />
    public override async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderPath = "")
    {
        // 生成唯一文件名
        var uniqueFileName = GenerateUniqueFileName(fileName);
        // 构建完整文件路径
        var filePath = BuildFilePath(folderPath, uniqueFileName);
        // 构建本地存储路径
        var localFilePath = Path.Combine(_config.LocalPath!, filePath.Replace('/', Path.DirectorySeparatorChar));
        // 确保目录存在
        var directoryPath = Path.GetDirectoryName(localFilePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        // 保存文件
        using var fileStreamOutput = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(fileStreamOutput);
        // 返回访问URL
        return $"{_config.BaseUrl}/{filePath}";
    }

    /// <inheritdoc />
    public override Task<Stream> DownloadFileAsync(string fileUrl)
    {
        // 从URL中提取文件路径
        var filePath = fileUrl.Replace($"{_config.BaseUrl}/", "");
        // 构建本地文件路径
        var localFilePath = Path.Combine(_config.LocalPath!, filePath.Replace('/', Path.DirectorySeparatorChar));
        // 检查文件是否存在
        if (!File.Exists(localFilePath))
        {
            throw new FileNotFoundException($"文件不存在: {localFilePath}");
        }
        // 返回文件流
        var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream>(fileStream);
    }

    /// <inheritdoc />
    public override Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            // 从URL中提取文件路径
            var filePath = fileUrl.Replace($"{_config.BaseUrl}/", "");
            // 构建本地文件路径
            var localFilePath = Path.Combine(_config.LocalPath!, filePath.Replace('/', Path.DirectorySeparatorChar));
            // 检查文件是否存在
            if (File.Exists(localFilePath))
            {
                // 删除文件
                File.Delete(localFilePath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <inheritdoc />
    public override Task<string> GetTemporaryUrlAsync(string filePath, int expiry = 3600)
    {
        // 对于本地存储，直接返回文件URL
        var fileUrl = $"{_config.BaseUrl}/{filePath}";
        return Task.FromResult(fileUrl);
    }

    /// <inheritdoc />
    public override Task<string> InitMultipartUploadAsync(Domain.Infrastructure.Services.File.UploadPartRequest request)
    {
        // 生成唯一的fileId
        var fileId = Guid.NewGuid().ToString();
        // 构建临时目录路径
        var tempDirectoryPath = Path.Combine(_config.LocalPath!, "temp", fileId);
        // 创建临时目录
        Directory.CreateDirectory(tempDirectoryPath);

        // 创建上传会话
        var session = new MultipartUploadSession
        {
            FileId = fileId,
            FileName = request.FileName,
            ContentType = request.ContentType,
            FolderPath = request.FolderPath,
            TotalParts = request.TotalParts,
            TotalSize = request.TotalSize,
            TempDirectoryPath = tempDirectoryPath
        };

        // 添加到会话字典
        _uploadSessions.TryAdd(fileId, session);

        return Task.FromResult(fileId);
    }

    /// <inheritdoc />
    public override async Task<Domain.Infrastructure.Services.File.UploadPartResponse> UploadPartAsync(string fileId, int partIndex, Stream fileStream)
    {
        // 获取上传会话
        if (!_uploadSessions.TryGetValue(fileId, out var session))
        {
            throw new ArgumentException("无效的fileId");
        }

        // 构建分片文件路径
        var partFilePath = Path.Combine(session.TempDirectoryPath ?? "", partIndex.ToString());
        // 保存分片文件
        using var fileStreamOutput = new FileStream(partFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await fileStream.CopyToAsync(fileStreamOutput);

        // 标记分片已上传
        session.UploadedParts.Add(partIndex);

        // 检查是否所有分片都已上传
        var isCompleted = session.UploadedParts.Count == session.TotalParts;

        return new Domain.Infrastructure.Services.File.UploadPartResponse
        {
            IsCompleted = isCompleted,
            FileUrl = null, // 不自动完成上传，由客户端手动调用CompleteMultipartUploadAsync
            UploadedParts = [.. session.UploadedParts.OrderBy(x => x)]
        };
    }

    /// <inheritdoc />
    public override async Task<string> CompleteMultipartUploadAsync(string fileId)
    {
        // 获取上传会话
        if (!_uploadSessions.TryRemove(fileId, out var session))
        {
            throw new ArgumentException("无效的fileId");
        }

        try
        {
            // 生成唯一文件名
            var uniqueFileName = GenerateUniqueFileName(session.FileName ?? "");
            // 构建完整文件路径
            var filePath = BuildFilePath(session.FolderPath ?? "", uniqueFileName);
            // 构建本地存储路径
            var localFilePath = Path.Combine(_config.LocalPath!, filePath.Replace('/', Path.DirectorySeparatorChar));
            // 确保目录存在
            var directoryPath = Path.GetDirectoryName(localFilePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 合并所有分片文件
            using var fileStreamOutput = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            for (var i = 0; i < session.TotalParts; i++)
            {
                var partFilePath = Path.Combine(session.TempDirectoryPath ?? "", i.ToString());
                if (File.Exists(partFilePath))
                {
                    using var partFileStream = new FileStream(partFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    await partFileStream.CopyToAsync(fileStreamOutput);
                }
            }

            // 删除临时目录和分片文件
            if (!string.IsNullOrEmpty(session.TempDirectoryPath) && Directory.Exists(session.TempDirectoryPath))
            {
                Directory.Delete(session.TempDirectoryPath, true);
            }

            // 返回访问URL
            return $"{_config.BaseUrl}/{filePath}";
        }
        catch
        {
            // 如果合并失败，删除临时目录和分片文件
            if (!string.IsNullOrEmpty(session.TempDirectoryPath) && Directory.Exists(session.TempDirectoryPath))
            {
                Directory.Delete(session.TempDirectoryPath, true);
            }
            throw;
        }
    }

    /// <inheritdoc />
    public override Task<bool> CancelMultipartUploadAsync(string fileId)
    {
        // 获取上传会话
        if (!_uploadSessions.TryRemove(fileId, out var session))
        {
            return Task.FromResult(false);
        }

        try
        {
            // 删除临时目录和分片文件
            if (!string.IsNullOrEmpty(session.TempDirectoryPath) && Directory.Exists(session.TempDirectoryPath))
            {
                Directory.Delete(session.TempDirectoryPath, true);
            }
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <inheritdoc />
    public override Task<List<int>> GetUploadedPartsAsync(string fileId)
    {
        // 获取上传会话
        if (!_uploadSessions.TryGetValue(fileId, out var session))
        {
            throw new ArgumentException("无效的fileId");
        }

        // 返回已上传的分片索引列表
        return Task.FromResult(session.UploadedParts.OrderBy(x => x).ToList());
    }
}