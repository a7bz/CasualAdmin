namespace CasualAdmin.Application.Services.System;
using CasualAdmin.Application.Interfaces.System;
using global::System.Security.Cryptography;
using global::System.Text;

/// <summary>
/// RSA加密服务
/// </summary>
public class RsaEncryptionService : IRsaEncryptionService
{
    private readonly RSA _rsa;
    private readonly string _publicKey;
    private readonly string _privateKey;

    /// <summary>
    /// 构造函数
    /// </summary>
    public RsaEncryptionService()
    {
        _rsa = RSA.Create();
        _rsa.KeySize = 2048;

        // 生成公钥和私钥
        _publicKey = ExportRsaPublicKeyToPem(_rsa); // 正确导出公钥
        _privateKey = _rsa.ToXmlString(true); // 包含私钥
    }

    /// <summary>
    /// 获取RSA公钥
    /// </summary>
    /// <returns>RSA公钥</returns>
    public string GetPublicKey()
    {
        return _publicKey;
    }

    /// <summary>
    /// 使用RSA私钥解密数据
    /// </summary>
    /// <param name="encryptedData">加密的数据（Base64编码）</param>
    /// <returns>解密后的明文字符串</returns>
    public string Decrypt(string encryptedData)
    {
        try
        {
            // 将Base64编码的加密数据转换为字节数组
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

            // 解密数据
            byte[] decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);

            // 将解密后的字节数组转换为字符串
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception)
        {
            throw new InvalidOperationException("密码解密失败");
        }
    }

    /// <summary>
    /// 导出RSA公钥到PEM格式
    /// </summary>
    /// <param name="rsa">RSA实例</param>
    /// <returns>PEM格式的RSA公钥</returns>
    private static string ExportRsaPublicKeyToPem(RSA rsa)
    {
        byte[] publicKeyBytes = rsa.ExportRSAPublicKey();
        return $"-----BEGIN RSA PUBLIC KEY-----\n" +
               $"{Convert.ToBase64String(publicKeyBytes, Base64FormattingOptions.InsertLineBreaks)}\n" +
               $"-----END RSA PUBLIC KEY-----";
    }
}