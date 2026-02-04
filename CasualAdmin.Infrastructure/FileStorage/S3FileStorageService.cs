namespace CasualAdmin.Infrastructure.FileStorage;

using System.Collections.Concurrent;
using Amazon.S3;
using Amazon.S3.Model;

/// <summary>
/// S3兼容文件存储服务
/// </summary>
public class S3FileStorageService : AbstractFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly ConcurrentDictionary<string, MultipartUploadSession> _uploadSessions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="config">文件存储配置</param>
    public S3FileStorageService(FileStorageConfig config)
        : base(config)
    {
        // 创建S3客户端配置
        var s3Config = new AmazonS3Config
        {
            ServiceURL = _config.Endpoint,
            ForcePathStyle = true // 兼容大部分S3服务提供商
        };

        // 初始化S3客户端
        _s3Client = new AmazonS3Client(
            _config.AccessKeyId,
            _config.AccessKeySecret,
            s3Config
        );

        // 初始化分片上传会话管理
        _uploadSessions = new ConcurrentDictionary<string, MultipartUploadSession>();
    }

    /// <summary>
    /// S3分片上传会话
    /// </summary>
    private class MultipartUploadSession
    {
        /// <summary>
        /// 上传ID
        /// </summary>
        public required string UploadId { get; set; }

        /// <summary>
        /// 已上传的分片信息
        /// </summary>
        public required Dictionary<int, string> UploadedParts { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        public required string ContentType { get; set; }

        /// <summary>
        /// 总分片数
        /// </summary>
        public int TotalParts { get; set; }
    }

    /// <inheritdoc />
    public override async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderPath = "")
    {
        // 生成唯一文件名
        var uniqueFileName = GenerateUniqueFileName(fileName);
        // 构建完整文件路径
        var objectKey = BuildFilePath(folderPath, uniqueFileName);

        // 上传文件请求
        var putRequest = new PutObjectRequest
        {
            BucketName = _config.BucketName,
            Key = objectKey,
            InputStream = fileStream,
            ContentType = contentType
        };

        // 执行上传
        await _s3Client.PutObjectAsync(putRequest);

        // 返回访问URL
        return $"{_config.BaseUrl}/{objectKey}";
    }

    /// <inheritdoc />
    public override async Task<Stream> DownloadFileAsync(string fileUrl)
    {
        // 从URL中提取对象键
        var objectKey = fileUrl.Replace($"{_config.BaseUrl}/", "");

        // 下载文件请求
        var getRequest = new GetObjectRequest
        {
            BucketName = _config.BucketName,
            Key = objectKey
        };

        // 执行下载
        var getResponse = await _s3Client.GetObjectAsync(getRequest);

        return getResponse.ResponseStream;
    }

    /// <inheritdoc />
    public override async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            // 从URL中提取对象键
            var objectKey = fileUrl.Replace($"{_config.BaseUrl}/", "");

            // 删除文件请求
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _config.BucketName,
                Key = objectKey
            };

            // 执行删除
            await _s3Client.DeleteObjectAsync(deleteRequest);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public override async Task<string> GetTemporaryUrlAsync(string filePath, int expiry = 3600)
    {
        // 生成预签名URL请求
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _config.BucketName,
            Key = filePath,
            Expires = DateTime.UtcNow.AddSeconds(expiry)
        };

        // 生成预签名URL
        var preSignedUrl = await _s3Client.GetPreSignedURLAsync(request);

        return preSignedUrl;
    }

    /// <inheritdoc />
    public override async Task<string> InitMultipartUploadAsync(Domain.Infrastructure.Services.File.UploadPartRequest request)
    {
        // 生成唯一文件名
        var uniqueFileName = GenerateUniqueFileName(request.FileName ?? "");
        // 构建完整文件路径
        var objectKey = BuildFilePath(request.FolderPath ?? "", uniqueFileName);

        // 初始化S3分片上传请求
        var initRequest = new InitiateMultipartUploadRequest
        {
            BucketName = _config.BucketName,
            Key = objectKey,
            ContentType = request.ContentType ?? "application/octet-stream"
        };

        // 执行初始化请求
        var initResponse = await _s3Client.InitiateMultipartUploadAsync(initRequest);

        // 创建并保存上传会话
        var session = new MultipartUploadSession
        {
            UploadId = initResponse.UploadId,
            UploadedParts = new Dictionary<int, string>(),
            ContentType = request.ContentType ?? "application/octet-stream",
            TotalParts = request.TotalParts
        };

        // S3分片上传的fileId使用对象键
        _uploadSessions[objectKey] = session;

        return objectKey;
    }

    /// <inheritdoc />
    public override async Task<Domain.Infrastructure.Services.File.UploadPartResponse> UploadPartAsync(string fileId, int partIndex, Stream fileStream)
    {
        // 检查上传会话是否存在
        if (!_uploadSessions.TryGetValue(fileId, out var session))
        {
            throw new KeyNotFoundException($"上传会话不存在：{fileId}");
        }

        // S3的PartNumber从1开始，而我们的partIndex从0开始，所以需要加1
        var partNumber = partIndex + 1;

        // 创建上传分片请求
        var uploadRequest = new UploadPartRequest
        {
            BucketName = _config.BucketName,
            Key = fileId,
            UploadId = session.UploadId,
            PartNumber = partNumber,
            InputStream = fileStream,
            IsLastPart = partNumber == session.TotalParts
        };

        // 执行分片上传
        var uploadResponse = await _s3Client.UploadPartAsync(uploadRequest);

        // 保存已上传的分片ETag
        session.UploadedParts[partIndex] = uploadResponse.ETag;

        // 检查是否所有分片都已上传完成
        var isCompleted = session.UploadedParts.Count == session.TotalParts;

        // 返回上传结果
        return new Domain.Infrastructure.Services.File.UploadPartResponse
        {
            IsCompleted = isCompleted,
            FileUrl = null, // 不自动完成上传，由客户端手动调用CompleteMultipartUploadAsync
            UploadedParts = session.UploadedParts.Keys.ToList()
        };
    }

    /// <inheritdoc />
    public override async Task<string> CompleteMultipartUploadAsync(string fileId)
    {
        // 检查上传会话是否存在
        if (!_uploadSessions.TryGetValue(fileId, out var session))
        {
            throw new KeyNotFoundException($"上传会话不存在：{fileId}");
        }

        // 创建完成上传请求
        var completeRequest = new CompleteMultipartUploadRequest
        {
            BucketName = _config.BucketName,
            Key = fileId,
            UploadId = session.UploadId,
            PartETags = session.UploadedParts
                .OrderBy(p => p.Key)
                .Select(p => new PartETag(p.Key + 1, p.Value))
                .ToList()
        };

        // 执行完成上传请求
        await _s3Client.CompleteMultipartUploadAsync(completeRequest);

        // 清除上传会话
        _uploadSessions.TryRemove(fileId, out _);

        // 返回文件访问URL
        return $"{_config.BaseUrl}/{fileId}";
    }

    /// <inheritdoc />
    public override async Task<bool> CancelMultipartUploadAsync(string fileId)
    {
        try
        {
            // 检查上传会话是否存在
            if (!_uploadSessions.TryGetValue(fileId, out var session))
            {
                return false;
            }

            // 创建取消上传请求
            var cancelRequest = new AbortMultipartUploadRequest
            {
                BucketName = _config.BucketName,
                Key = fileId,
                UploadId = session.UploadId
            };

            // 执行取消上传请求
            await _s3Client.AbortMultipartUploadAsync(cancelRequest);

            // 清除上传会话
            _uploadSessions.TryRemove(fileId, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public override async Task<List<int>> GetUploadedPartsAsync(string fileId)
    {
        // 检查上传会话是否存在
        if (!_uploadSessions.TryGetValue(fileId, out var session))
        {
            // 如果会话不存在，可能是因为服务重启或会话超时
            // 此时需要从S3服务器查询已上传的分片
            return await QueryUploadedPartsFromS3Async(fileId);
        }

        // 从本地会话中返回已上传的分片索引
        return session.UploadedParts.Keys.OrderBy(k => k).ToList();
    }

    /// <summary>
    /// 从S3服务器查询已上传的分片
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <returns>已上传的分片索引列表</returns>
    private Task<List<int>> QueryUploadedPartsFromS3Async(string fileId)
    {
        // 这里需要实现从S3查询已上传分片的逻辑
        // 由于当前设计中，我们没有在服务重启后保存UploadId，所以暂时返回空列表
        // 在生产环境中，应该将UploadId持久化存储，以便服务重启后能够恢复上传会话
        return Task.FromResult(new List<int>());
    }
}