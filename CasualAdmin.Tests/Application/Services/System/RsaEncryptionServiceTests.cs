namespace CasualAdmin.Tests.Application.Services.System;
using System;
using CasualAdmin.Application.Services.System;
using Xunit;

/// <summary>
/// RSA加密服务测试类
/// </summary>
public class RsaEncryptionServiceTests
{
    private readonly RsaEncryptionService _rsaEncryptionService;

    /// <summary>
    /// 测试类构造函数，初始化被测服务实例
    /// </summary>
    public RsaEncryptionServiceTests()
    {
        // 创建被测服务实例
        _rsaEncryptionService = new RsaEncryptionService();
    }

    /// <summary>
    /// 测试获取RSA公钥方法，确保返回非空公钥字符串
    /// 公钥字符串应包含<RSAKeyValue>标签
    /// </summary>
    [Fact]
    public void GetPublicKey_ShouldReturnValidPublicKey()
    {
        // Act
        var publicKey = _rsaEncryptionService.GetPublicKey();

        // Assert
        Assert.NotNull(publicKey);
        Assert.NotEmpty(publicKey);
        Assert.Contains("<RSAKeyValue>", publicKey);
    }

    /// <summary>
    /// 测试解密方法，确保抛出InvalidOperationException异常
    /// 注意：这里我们不测试实际的解密功能，因为需要有效的加密数据和对应的私钥
    /// 只测试当传入无效数据时是否抛出正确的异常
    /// </summary>
    [Fact]
    public void Decrypt_ShouldThrowInvalidOperationException_WhenInvalidData()
    {
        // Arrange
        var invalidEncryptedData = "invalid_encrypted_data";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _rsaEncryptionService.Decrypt(invalidEncryptedData));
    }
}
