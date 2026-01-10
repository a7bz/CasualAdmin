namespace CasualAdmin.Application.Models.File;

/// <summary>
/// 分片上传请求参数
/// </summary>
public class UploadPartRequest
{
    /// <summary>
    /// 文件唯一标识
    /// </summary>
    public string? FileId { get; set; }

    /// <summary>
    /// 分片索引
    /// </summary>
    public int PartIndex { get; set; }

    /// <summary>
    /// 分片总数
    /// </summary>
    public int TotalParts { get; set; }

    /// <summary>
    /// 分片大小
    /// </summary>
    public long PartSize { get; set; }

    /// <summary>
    /// 文件总大小
    /// </summary>
    public long TotalSize { get; set; }

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
}