namespace CasualAdmin.Domain.Infrastructure.Services;
using CasualAdmin.Domain.Infrastructure.Services.File;

/// <summary>
/// 文件存储服务接口
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <param name="fileName">文件名</param>
    /// <param name="contentType">文件内容类型</param>
    /// <param name="folderPath">存储路径</param>
    /// <returns>文件访问URL</returns>
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folderPath = "");

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="fileUrl">文件URL</param>
    /// <returns>文件流</returns>
    Task<Stream> DownloadFileAsync(string fileUrl);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileUrl">文件URL</param>
    /// <returns>删除结果</returns>
    Task<bool> DeleteFileAsync(string fileUrl);

    /// <summary>
    /// 获取文件临时访问URL
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="expiry">过期时间（秒）</param>
    /// <returns>临时访问URL</returns>
    Task<string> GetTemporaryUrlAsync(string filePath, int expiry = 3600);

    /// <summary>
    /// 初始化分片上传
    /// </summary>
    /// <param name="request">分片上传请求</param>
    /// <returns>文件唯一标识</returns>
    Task<string> InitMultipartUploadAsync(UploadPartRequest request);

    /// <summary>
    /// 上传分片
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <param name="partIndex">分片索引</param>
    /// <param name="fileStream">分片流</param>
    /// <returns>分片上传结果</returns>
    Task<UploadPartResponse> UploadPartAsync(string fileId, int partIndex, Stream fileStream);

    /// <summary>
    /// 完成分片上传
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <returns>文件访问URL</returns>
    Task<string> CompleteMultipartUploadAsync(string fileId);

    /// <summary>
    /// 取消分片上传
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <returns>取消结果</returns>
    Task<bool> CancelMultipartUploadAsync(string fileId);

    /// <summary>
    /// 查询已上传的分片
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <returns>已上传的分片索引列表</returns>
    Task<List<int>> GetUploadedPartsAsync(string fileId);
}