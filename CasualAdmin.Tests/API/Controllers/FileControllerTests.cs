namespace CasualAdmin.Tests.API.Controllers;

using CasualAdmin.API.Controllers;
using CasualAdmin.Domain.Infrastructure.Services;
using global::System.IO;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

/// <summary>
/// 文件控制器测试
/// </summary>
public class FileControllerTests
{
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly FileController _fileController;

    /// <summary>
    /// 构造函数，初始化模拟对象和被测控制器
    /// </summary>
    public FileControllerTests()
    {
        // 初始化模拟对象
        _fileStorageServiceMock = new Mock<IFileStorageService>();

        // 创建被测控制器实例
        _fileController = new FileController(_fileStorageServiceMock.Object);
    }

    /// <summary>
    /// 测试上传文件方法，当文件不为空时返回成功结果
    /// </summary>
    [Fact]
    public async Task Upload_ShouldReturnSuccessResult_WhenFileIsNotNull()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = new MemoryStream(global::System.Text.Encoding.UTF8.GetBytes("test file content"));
        var fileName = "test.txt";
        var contentType = "text/plain";

        fileMock.Setup(f => f.OpenReadStream()).Returns(content);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.Length).Returns(content.Length);

        _fileStorageServiceMock.Setup(service => service.UploadFileAsync(It.IsAny<Stream>(), fileName, contentType, It.IsAny<string>())).ReturnsAsync("https://example.com/files/test.txt");

        // Act
        var result = await _fileController.Upload(fileMock.Object, "test-folder");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
        Assert.Equal("https://example.com/files/test.txt", result.Data);
    }

    /// <summary>
    /// 测试上传文件方法，当文件为空时返回BadRequest结果
    /// </summary>
    [Fact]
    public async Task Upload_ShouldReturnBadRequestResult_WhenFileIsEmpty()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = new MemoryStream();

        fileMock.Setup(f => f.OpenReadStream()).Returns(content);
        fileMock.Setup(f => f.Length).Returns(content.Length);

        // Act
        var result = await _fileController.Upload(fileMock.Object, "test-folder");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.Code);
    }

    /// <summary>
    /// 测试删除文件方法，当文件URL不为空时返回成功结果
    /// </summary>
    [Fact]
    public async Task Delete_ShouldReturnSuccessResult_WhenFileUrlIsNotNull()
    {
        // Arrange
        var fileUrl = "https://example.com/files/test.txt";

        _fileStorageServiceMock.Setup(service => service.DeleteFileAsync(fileUrl)).ReturnsAsync(true);

        // Act
        var result = await _fileController.Delete(fileUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试删除文件方法，当文件URL为空时返回BadRequest结果
    /// </summary>
    [Fact]
    public async Task Delete_ShouldReturnBadRequestResult_WhenFileUrlIsEmpty()
    {
        // Act
        var result = await _fileController.Delete(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.Code);
    }

    /// <summary>
    /// 测试获取文件临时访问URL方法，当文件路径不为空时返回成功结果
    /// </summary>
    [Fact]
    public async Task GetTemporaryUrl_ShouldReturnSuccessResult_WhenFilePathIsNotNull()
    {
        // Arrange
        var filePath = "test-folder/test.txt";

        _fileStorageServiceMock.Setup(service => service.GetTemporaryUrlAsync(filePath, It.IsAny<int>())).ReturnsAsync("https://example.com/files/test.txt?token=temp-token");

        // Act
        var result = await _fileController.GetTemporaryUrl(filePath, 3600);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
        Assert.Equal("https://example.com/files/test.txt?token=temp-token", result.Data);
    }

    /// <summary>
    /// 测试获取文件临时访问URL方法，当文件路径为空时返回BadRequest结果
    /// </summary>
    [Fact]
    public async Task GetTemporaryUrl_ShouldReturnBadRequestResult_WhenFilePathIsEmpty()
    {
        // Act
        var result = await _fileController.GetTemporaryUrl(string.Empty, 3600);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.Code);
    }

    /// <summary>
    /// 测试初始化分片上传方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task InitMultipartUpload_ShouldReturnSuccessResult()
    {
        // Arrange
        var uploadPartRequest = new Domain.Infrastructure.Services.File.UploadPartRequest
        {
            FileName = "test.txt",
            ContentType = "text/plain",
            TotalSize = 1024,
            TotalParts = 2,
            PartSize = 512
        };

        _fileStorageServiceMock.Setup(service => service.InitMultipartUploadAsync(uploadPartRequest)).ReturnsAsync("file-12345");

        // Act
        var result = await _fileController.InitMultipartUpload(uploadPartRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
        Assert.Equal("file-12345", result.Data);
    }

    /// <summary>
    /// 测试上传分片方法，当文件不为空时返回成功结果
    /// </summary>
    [Fact]
    public async Task UploadPart_ShouldReturnSuccessResult_WhenFileIsNotNull()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = new MemoryStream(global::System.Text.Encoding.UTF8.GetBytes("test part content"));
        var fileName = "test.txt";
        var contentType = "text/plain";

        fileMock.Setup(f => f.OpenReadStream()).Returns(content);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.Length).Returns(content.Length);

        var uploadPartResponse = new Domain.Infrastructure.Services.File.UploadPartResponse
        {
            IsCompleted = false,
            UploadedParts = new List<int> { 0 }
        };

        _fileStorageServiceMock.Setup(service => service.UploadPartAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Stream>())).ReturnsAsync(uploadPartResponse);

        // Act
        var result = await _fileController.UploadPart("file-12345", 0, fileMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
        Assert.False(result.Data.IsCompleted);
        Assert.NotNull(result.Data.UploadedParts);
        Assert.Contains(0, result.Data.UploadedParts);
    }

    /// <summary>
    /// 测试上传分片方法，当文件为空时返回BadRequest结果
    /// </summary>
    [Fact]
    public async Task UploadPart_ShouldReturnBadRequestResult_WhenFileIsEmpty()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = new MemoryStream();

        fileMock.Setup(f => f.OpenReadStream()).Returns(content);
        fileMock.Setup(f => f.Length).Returns(content.Length);

        // Act
        var result = await _fileController.UploadPart("file-12345", 0, fileMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.Code);
    }

    /// <summary>
    /// 测试完成分片上传方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CompleteMultipartUpload_ShouldReturnSuccessResult()
    {
        // Arrange
        var fileId = "file-12345";

        _fileStorageServiceMock.Setup(service => service.CompleteMultipartUploadAsync(fileId)).ReturnsAsync("https://example.com/files/test.txt");

        // Act
        var result = await _fileController.CompleteMultipartUpload(fileId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
        Assert.Equal("https://example.com/files/test.txt", result.Data);
    }

    /// <summary>
    /// 测试取消分片上传方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task CancelMultipartUpload_ShouldReturnSuccessResult()
    {
        // Arrange
        var fileId = "file-12345";

        _fileStorageServiceMock.Setup(service => service.CancelMultipartUploadAsync(fileId)).ReturnsAsync(true);

        // Act
        var result = await _fileController.CancelMultipartUpload(fileId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.True(result.Data);
    }

    /// <summary>
    /// 测试查询已上传的分片方法，返回成功结果
    /// </summary>
    [Fact]
    public async Task GetUploadedParts_ShouldReturnSuccessResult()
    {
        // Arrange
        var fileId = "file-12345";
        var uploadedParts = new List<int> { 0, 1 };

        _fileStorageServiceMock.Setup(service => service.GetUploadedPartsAsync(fileId)).ReturnsAsync(uploadedParts);

        // Act
        var result = await _fileController.GetUploadedParts(fileId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Code);
        Assert.NotNull(result.Data);
        Assert.Equal(uploadedParts, result.Data);
    }
}