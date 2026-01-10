namespace CasualAdmin.Tests.Infrastructure.FileStorage;
using System.IO;
using System.Text;
using CasualAdmin.Domain.Entities;
using CasualAdmin.Infrastructure.FileStorage;
using Xunit;

/// <summary>
/// 本地文件存储服务测试类
/// 验证文件上传、下载、删除等操作是否按预期进行
/// 测试环境中使用临时目录存储文件，确保测试间数据隔离
/// </summary>
public class LocalFileStorageServiceTests
{
    private LocalFileStorageService _storageService;
    private string _tempDirectory;
    private FileStorageConfig _config;

    /// <summary>
    /// 测试类构造函数，初始化测试环境
    /// 创建临时目录用于存储测试文件
    /// 配置本地文件存储服务，指定临时目录作为存储路径
    /// </summary>
    public LocalFileStorageServiceTests()
    {
        // 创建临时目录
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);

        // 配置文件存储
        _config = new FileStorageConfig
        {
            Type = FileStorageType.Local,
            LocalPath = _tempDirectory,
            BaseUrl = "http://localhost:5001/files"
        };

        // 创建测试服务实例
        _storageService = new LocalFileStorageService(_config);
    }

    /// <summary>
    /// 测试结束后清理临时目录
    /// 确保每个测试都在独立的环境中运行，避免数据污染
    /// </summary>
    ~LocalFileStorageServiceTests()
    {
        // 清理临时目录
        if (Directory.Exists(_tempDirectory))
        {
            try
            {
                Directory.Delete(_tempDirectory, true);
            }
            catch { }
        }
    }

    /// <summary>
    /// 测试普通文件上传功能
    /// 验证文件能够成功上传到指定目录
    /// 验证返回的文件URL是否符合预期格式
    /// 验证上传后的文件是否存在且内容正确
    /// </summary>
    [Fact]
    public async Task UploadFileAsync_ShouldUploadFileSuccessfully()
    {
        // Arrange
        var fileContent = "Hello, World!";
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var fileName = "test.txt";
        var contentType = "text/plain";

        // Act
        var fileUrl = await _storageService.UploadFileAsync(fileStream, fileName, contentType);

        // Assert
        Assert.NotNull(fileUrl);
        Assert.NotEmpty(fileUrl);
        Assert.StartsWith(_config.BaseUrl!, fileUrl);

        // 验证文件是否存在
        var localFilePath = Path.Combine(_tempDirectory, fileUrl.Replace($"{_config.BaseUrl}/", "").Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(localFilePath));

        // 验证文件内容
        var readContent = await File.ReadAllTextAsync(localFilePath);
        Assert.Equal(fileContent, readContent);
    }

    /// <summary>
    /// 测试文件下载功能
    /// </summary>
    [Fact]
    public async Task DownloadFileAsync_ShouldDownloadFileSuccessfully()
    {
        // Arrange
        var fileContent = "Test file content for download";
        var fileName = "download.txt";
        var contentType = "text/plain";

        // 先上传文件
        var fileUrl = await _storageService.UploadFileAsync(
            new MemoryStream(Encoding.UTF8.GetBytes(fileContent)),
            fileName,
            contentType
        );

        // Act
        var downloadStream = await _storageService.DownloadFileAsync(fileUrl);

        // Assert
        Assert.NotNull(downloadStream);

        // 验证下载内容
        using (var reader = new StreamReader(downloadStream))
        {
            var readContent = await reader.ReadToEndAsync();
            Assert.Equal(fileContent, readContent);
        }
    }

    /// <summary>
    /// 测试文件删除功能
    /// </summary>
    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFileSuccessfully()
    {
        // Arrange
        var fileContent = "File to be deleted";
        var fileName = "delete.txt";
        var contentType = "text/plain";

        // 先上传文件
        var fileUrl = await _storageService.UploadFileAsync(
            new MemoryStream(Encoding.UTF8.GetBytes(fileContent)),
            fileName,
            contentType
        );

        // 获取本地文件路径
        var localFilePath = Path.Combine(_tempDirectory, fileUrl.Replace($"{_config.BaseUrl}/", "").Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(localFilePath));

        // Act
        var result = await _storageService.DeleteFileAsync(fileUrl);

        // Assert
        Assert.True(result);
        Assert.False(File.Exists(localFilePath));
    }

    /// <summary>
    /// 测试获取临时URL功能
    /// </summary>
    [Fact]
    public async Task GetTemporaryUrlAsync_ShouldReturnValidUrl()
    {
        // Arrange
        var filePath = "test/temp-url.txt";

        // Act
        var tempUrl = await _storageService.GetTemporaryUrlAsync(filePath);

        // Assert
        Assert.NotNull(tempUrl);
        Assert.NotEmpty(tempUrl);
        Assert.StartsWith(_config.BaseUrl!, tempUrl);
        Assert.EndsWith(filePath, tempUrl);
    }

    /// <summary>
    /// 测试分片上传完整流程
    /// </summary>
    [Fact]
    public async Task MultipartUpload_ShouldWorkCorrectly()
    {
        // Arrange
        var totalParts = 3;
        var fileContent = "This is a test file for multipart upload functionality.";
        var fileName = "multipart.txt";
        var contentType = "text/plain";
        var folderPath = "multipart";

        // 准备分片数据
        var fileBytes = Encoding.UTF8.GetBytes(fileContent);
        var partSize = (int)Math.Ceiling((double)fileBytes.Length / totalParts);
        var parts = new List<byte[]>();
        for (var i = 0; i < totalParts; i++)
        {
            var start = i * partSize;
            var length = Math.Min(partSize, fileBytes.Length - start);
            var partBytes = new byte[length];
            Array.Copy(fileBytes, start, partBytes, 0, length);
            parts.Add(partBytes);
        }

        // Act - 1. 初始化分片上传
        var uploadRequest = new CasualAdmin.Application.Models.File.UploadPartRequest
        {
            FileName = fileName,
            ContentType = contentType,
            TotalParts = totalParts,
            TotalSize = fileBytes.Length,
            FolderPath = folderPath
        };
        var fileId = await _storageService.InitMultipartUploadAsync(uploadRequest);

        // Act - 2. 上传所有分片
        foreach (var (part, index) in parts.Select((p, i) => (p, i)))
        {
            using var partStream = new MemoryStream(part);
            await _storageService.UploadPartAsync(fileId, index, partStream);
        }

        // Act - 3. 完成分片上传
        var finalUrl = await _storageService.CompleteMultipartUploadAsync(fileId);

        // Assert - 验证最终文件
        Assert.NotNull(finalUrl);
        Assert.NotEmpty(finalUrl);
        Assert.StartsWith(_config.BaseUrl!, finalUrl);

        // 下载文件并验证内容
        using var downloadStream = await _storageService.DownloadFileAsync(finalUrl);
        using var reader = new StreamReader(downloadStream);
        var downloadedContent = await reader.ReadToEndAsync();
        Assert.Equal(fileContent, downloadedContent);
    }

    /// <summary>
    /// 测试查询已上传的分片
    /// </summary>
    [Fact]
    public async Task GetUploadedPartsAsync_ShouldReturnCorrectParts()
    {
        // Arrange
        var totalParts = 5;
        var fileName = "uploaded-parts.txt";
        var contentType = "text/plain";

        // 初始化分片上传
        var uploadRequest = new CasualAdmin.Application.Models.File.UploadPartRequest
        {
            FileName = fileName,
            ContentType = contentType,
            TotalParts = totalParts,
            TotalSize = 1000,
            FolderPath = "test"
        };
        var fileId = await _storageService.InitMultipartUploadAsync(uploadRequest);

        // 上传部分分片
        var uploadedParts = new List<int> { 0, 2, 4 };
        foreach (var partIndex in uploadedParts)
        {
            using var partStream = new MemoryStream(new byte[100]);
            await _storageService.UploadPartAsync(fileId, partIndex, partStream);
        }

        // Act
        var result = await _storageService.GetUploadedPartsAsync(fileId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(uploadedParts.Count, result.Count);
        var sortedUploadedParts = uploadedParts.OrderBy(x => x).ToList();
        for (int i = 0; i < sortedUploadedParts.Count; i++)
        {
            Assert.Equal(sortedUploadedParts[i], result[i]);
        }

        // 清理
        await _storageService.CancelMultipartUploadAsync(fileId);
    }

    /// <summary>
    /// 测试取消分片上传
    /// </summary>
    [Fact]
    public async Task CancelMultipartUploadAsync_ShouldCleanupResources()
    {
        // Arrange
        var fileName = "cancel-upload.txt";
        var contentType = "text/plain";

        // 初始化分片上传
        var uploadRequest = new CasualAdmin.Application.Models.File.UploadPartRequest
        {
            FileName = fileName,
            ContentType = contentType,
            TotalParts = 3,
            TotalSize = 300,
            FolderPath = "test"
        };
        var fileId = await _storageService.InitMultipartUploadAsync(uploadRequest);

        // 上传一些分片
        using var partStream = new MemoryStream(new byte[100]);
        await _storageService.UploadPartAsync(fileId, 0, partStream);

        // 验证临时目录路径
        var tempDirPath = Path.Combine(_config.LocalPath!, "temp", fileId);
        Assert.True(Directory.Exists(tempDirPath));

        // Act
        var result = await _storageService.CancelMultipartUploadAsync(fileId);

        // Assert
        Assert.True(result);
        Assert.False(Directory.Exists(tempDirPath));
    }

    /// <summary>
    /// 测试上传到指定文件夹
    /// </summary>
    [Fact]
    public async Task UploadFileAsync_ShouldUploadToSpecifiedFolder()
    {
        // Arrange
        var fileContent = "File in custom folder";
        var fileName = "folder-test.txt";
        var contentType = "text/plain";
        var customFolder = "custom-folder";

        // Act
        var fileUrl = await _storageService.UploadFileAsync(
            new MemoryStream(Encoding.UTF8.GetBytes(fileContent)),
            fileName,
            contentType,
            customFolder
        );

        // Assert
        Assert.NotNull(fileUrl);
        Assert.NotEmpty(fileUrl);
        Assert.Contains(customFolder, fileUrl);

        // 验证文件路径
        var localFilePath = Path.Combine(_tempDirectory, fileUrl.Replace($"{_config.BaseUrl}/", "").Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(localFilePath));

        // 验证文件内容
        var readContent = await File.ReadAllTextAsync(localFilePath);
        Assert.Equal(fileContent, readContent);
    }
}
