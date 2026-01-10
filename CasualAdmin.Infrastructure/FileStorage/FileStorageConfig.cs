namespace CasualAdmin.Infrastructure.FileStorage;

/// <summary>
/// 文件存储类型枚举
/// </summary>
public enum FileStorageType
{
    /// <summary>
    /// 本地存储
    /// </summary>
    Local = 0,

    /// <summary>
    /// S3兼容存储（支持阿里云OSS、腾讯云COS、华为云OBS、Amazon S3等）
    /// </summary>
    S3 = 1
}

/// <summary>
/// 文件存储配置
/// </summary>
public class FileStorageConfig
{
    /// <summary>
    /// 存储类型
    /// </summary>
    public FileStorageType Type { get; set; }

    /// <summary>
    /// 访问密钥ID
    /// </summary>
    public string? AccessKeyId { get; set; }

    /// <summary>
    /// 访问密钥密钥
    /// </summary>
    public string? AccessKeySecret { get; set; }

    /// <summary>
    /// 存储桶名称
    /// </summary>
    public string? BucketName { get; set; }

    /// <summary>
    /// 地域
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// 域名
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// 本地存储路径
    /// </summary>
    public string? LocalPath { get; set; }

    /// <summary>
    /// 基础URL
    /// </summary>
    public string? BaseUrl { get; set; }
}