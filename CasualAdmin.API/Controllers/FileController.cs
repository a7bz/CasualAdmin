namespace CasualAdmin.API.Controllers;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Models.File;
using CasualAdmin.Shared.Common;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// 文件管理控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="fileStorageService">文件存储服务</param>
    public FileController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">要上传的文件</param>
    /// <param name="folderPath">存储路径</param>
    /// <returns>文件访问URL</returns>
    [HttpPost("upload")]
    public async Task<ApiResponse<string>> Upload(IFormFile file, [FromForm] string folderPath = "")
    {
        if (file == null || file.Length == 0)
        {
            return ApiResponse<string>.BadRequest("请选择要上传的文件");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var fileUrl = await _fileStorageService.UploadFileAsync(
                stream,
                file.FileName,
                file.ContentType,
                folderPath
            );

            return ApiResponse<string>.Success(fileUrl, "文件上传成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Failed("文件上传失败: " + ex.Message, 500);
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="fileUrl">文件URL</param>
    /// <returns>文件流</returns>
    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return BadRequest(new { message = "文件URL不能为空" });
        }

        try
        {
            var stream = await _fileStorageService.DownloadFileAsync(fileUrl);
            var fileName = Path.GetFileName(fileUrl);

            return File(stream, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "文件下载失败", error = ex.Message });
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="fileUrl">文件URL</param>
    /// <returns>删除结果</returns>
    [HttpDelete("delete")]
    public async Task<ApiResponse<bool>> Delete([FromQuery] string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return ApiResponse<bool>.BadRequest("文件URL不能为空");
        }

        try
        {
            var result = await _fileStorageService.DeleteFileAsync(fileUrl);
            return ApiResponse<bool>.Success(result, "文件删除成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failed("文件删除失败: " + ex.Message, 500);
        }
    }

    /// <summary>
    /// 获取文件临时访问URL
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="expiry">过期时间（秒）</param>
    /// <returns>临时访问URL</returns>
    [HttpGet("temporary-url")]
    public async Task<ApiResponse<string>> GetTemporaryUrl([FromQuery] string filePath, [FromQuery] int expiry = 3600)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return ApiResponse<string>.BadRequest("文件路径不能为空");
        }

        try
        {
            var temporaryUrl = await _fileStorageService.GetTemporaryUrlAsync(filePath, expiry);
            return ApiResponse<string>.Success(temporaryUrl, "获取临时URL成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Failed("获取临时URL失败: " + ex.Message, 500);
        }
    }

    /// <summary>
    /// 初始化分片上传
    /// </summary>
    /// <param name="request">分片上传请求参数</param>
    /// <returns>文件唯一标识</returns>
    [HttpPost("multipart/init")]
    public async Task<ApiResponse<string>> InitMultipartUpload([FromBody] UploadPartRequest request)
    {
        try
        {
            var fileId = await _fileStorageService.InitMultipartUploadAsync(request);
            return ApiResponse<string>.Success(fileId, "初始化分片上传成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Failed("初始化分片上传失败: " + ex.Message, 500);
        }
    }

    /// <summary>
    /// 上传分片
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <param name="partIndex">分片索引</param>
    /// <param name="file">分片文件</param>
    /// <returns>分片上传结果</returns>
    [HttpPost("multipart/upload")]
    public async Task<ApiResponse<UploadPartResponse>> UploadPart(string fileId, int partIndex, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return ApiResponse<UploadPartResponse>.BadRequest("请选择要上传的分片文件");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var response = await _fileStorageService.UploadPartAsync(fileId, partIndex, stream);
            return ApiResponse<UploadPartResponse>.Success(response, "分片上传成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<UploadPartResponse>.Failed("分片上传失败: " + ex.Message, 500);
        }
    }

    /// <summary>
    /// 完成分片上传
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <returns>文件访问URL</returns>
    [HttpPost("multipart/complete")]
    public async Task<ApiResponse<string>> CompleteMultipartUpload([FromQuery] string fileId)
    {
        try
        {
            var fileUrl = await _fileStorageService.CompleteMultipartUploadAsync(fileId);
            return ApiResponse<string>.Success(fileUrl, "完成分片上传成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.Failed("完成分片上传失败: " + ex.Message, 500);
        }
    }

    /// <summary>
    /// 取消分片上传
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <returns>取消结果</returns>
    [HttpPost("multipart/cancel")]
    public async Task<ApiResponse<bool>> CancelMultipartUpload([FromQuery] string fileId)
    {
        try
        {
            var result = await _fileStorageService.CancelMultipartUploadAsync(fileId);
            return ApiResponse<bool>.Success(result, "取消分片上传成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Failed("取消分片上传失败: " + ex.Message, 500);
        }
    }

    /// <summary>
    /// 查询已上传的分片
    /// </summary>
    /// <param name="fileId">文件唯一标识</param>
    /// <returns>已上传的分片索引列表</returns>
    [HttpGet("multipart/parts")]
    public async Task<ApiResponse<List<int>>> GetUploadedParts([FromQuery] string fileId)
    {
        try
        {
            var uploadedParts = await _fileStorageService.GetUploadedPartsAsync(fileId);
            return ApiResponse<List<int>>.Success(uploadedParts, "查询已上传分片成功");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<int>>.Failed("查询已上传分片失败: " + ex.Message, 500);
        }
    }
}