namespace CasualAdmin.Application.Interfaces.System;

/// <summary>
/// RSA加密服务接口
/// </summary>
public interface IRsaEncryptionService
{
    /// <summary>
    /// 获取RSA公钥
    /// </summary>
    /// <returns>RSA公钥</returns>
    string GetPublicKey();

    /// <summary>
    /// 使用RSA私钥解密数据
    /// </summary>
    /// <param name="encryptedData">加密的数据</param>
    /// <returns>解密后的数据</returns>
    string Decrypt(string encryptedData);
}