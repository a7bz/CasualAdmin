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
        _publicKey = _rsa.ToXmlString(false); // 只包含公钥的XML格式
        _privateKey = _rsa.ToXmlString(true); // 包含私钥
    }

    /// <summary>
    /// 获取RSA公钥
    /// </summary>
    /// <returns>RSA公钥</returns>
    public string GetPublicKey()
    {
        return ExportPublicKeySpki();
    }

    /// <summary>
    /// 导出SPKI格式公钥
    /// </summary>
    private string ExportPublicKeySpki()
    {
        // ExportSubjectPublicKeyInfo 返回 SPKI 格式
        byte[] publicKeyBytes = _rsa.ExportSubjectPublicKeyInfo();
        string base64Key = Convert.ToBase64String(publicKeyBytes);

        // 格式化为 PEM
        return FormatPem(base64Key, "PUBLIC KEY");
    }

    /// <summary>
    /// 导出传统RSA公钥格式
    /// </summary>
    private string ExportRsaPublicKey()
    {
        // ExportRSAPublicKey 返回 PKCS#1 格式
        byte[] publicKeyBytes = _rsa.ExportRSAPublicKey();
        string base64Key = Convert.ToBase64String(publicKeyBytes);

        // 格式化为 PEM
        return FormatPem(base64Key, "RSA PUBLIC KEY");
    }

    /// <summary>
    /// 格式化PEM密钥
    /// </summary>
    private string FormatPem(string base64Key, string keyType)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"-----BEGIN {keyType}-----");

        // 每64个字符换行
        for (int i = 0; i < base64Key.Length; i += 64)
        {
            int length = Math.Min(64, base64Key.Length - i);
            sb.AppendLine(base64Key.Substring(i, length));
        }

        sb.AppendLine($"-----END {keyType}-----");
        return sb.ToString();
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
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] decryptedBytes;

            // OAEP-SHA256 填充（Web Crypto API 使用）
            decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("加密数据不是有效的Base64格式");
        }
        catch (CryptographicException ex)
        {
            throw new InvalidOperationException($"密码解密失败: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"解密过程中发生错误: {ex.Message}");
        }
    }
}