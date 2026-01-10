namespace CasualAdmin.Infrastructure.FileStorage;
using CasualAdmin.Application.Interfaces.Services;

/// <summary>
/// 文件存储抽象基类
/// </summary>
public abstract class AbstractFileStorageService : IFileStorageService
{
    /// <summary>
    /// 文件存储配置
    /// </summary>
    protected readonly FileStorageConfig _config;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="config">文件存储配置</param>
    protected AbstractFileStorageService(FileStorageConfig config)
    {
        _config = config;
    }

    /// <inheritdoc />
    public abstract Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderPath = "");

    /// <inheritdoc />
    public abstract Task<Stream> DownloadFileAsync(string fileUrl);

    /// <inheritdoc />
    public abstract Task<bool> DeleteFileAsync(string fileUrl);

    /// <inheritdoc />
    public abstract Task<string> GetTemporaryUrlAsync(string filePath, int expiry = 3600);

    /// <inheritdoc />
    public abstract Task<string> InitMultipartUploadAsync(Application.Models.File.UploadPartRequest request);

    /// <inheritdoc />
    public abstract Task<Application.Models.File.UploadPartResponse> UploadPartAsync(string fileId, int partIndex, Stream fileStream);

    /// <inheritdoc />
    public abstract Task<string> CompleteMultipartUploadAsync(string fileId);

    /// <inheritdoc />
    public abstract Task<bool> CancelMultipartUploadAsync(string fileId);

    /// <inheritdoc />
    public abstract Task<List<int>> GetUploadedPartsAsync(string fileId);

    /// <summary>
    /// 生成唯一文件名
    /// </summary>
    /// <param name="originalFileName">原始文件名</param>
    /// <returns>唯一文件名</returns>
    protected virtual string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var uniqueName = $"{Guid.NewGuid()}{extension}";
        return uniqueName;
    }

    /// <summary>
    /// 构建文件路径
    /// </summary>
    /// <param name="folderPath">文件夹路径</param>
    /// <param name="fileName">文件名</param>
    /// <returns>完整文件路径</returns>
    protected virtual string BuildFilePath(string folderPath, string fileName)
    {
        // 获取当前年月，格式：yyyy/MM
        var currentMonth = DateTime.Now.ToString("yyyy/MM");

        // 构建基础路径：自定义路径/年月
        var basePath = string.IsNullOrWhiteSpace(folderPath)
            ? currentMonth
            : $"{folderPath.TrimEnd('/')}/{currentMonth}";

        return $"{basePath}/{fileName}";
    }
}