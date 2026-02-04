namespace CasualAdmin.Domain.Infrastructure.Services.File;

using System.Collections.Generic;

/// <summary>
/// 分片上传响应
/// </summary>
public class UploadPartResponse
{
    /// <summary>
    /// 是否上传完成
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// 文件访问URL
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// 已上传的分片索引列表
    /// </summary>
    public List<int>? UploadedParts { get; set; }
}