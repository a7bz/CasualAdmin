namespace CasualAdmin.Infrastructure.Cache;

/// <summary>
/// 缓存配置选项
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// 是否启用Redis，false则使用内存缓存
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Redis配置
    /// </summary>
    public RedisOptions Redis { get; set; } = new();
}

/// <summary>
/// Redis配置选项
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Redis主机地址
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Redis端口
    /// </summary>
    public int Port { get; set; } = 6379;

    /// <summary>
    /// Redis密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 数据库索引
    /// </summary>
    public int Database { get; set; } = 0;

    /// <summary>
    /// 键前缀
    /// </summary>
    public string KeyPrefix { get; set; } = "casualadmin:";

    /// <summary>
    /// 是否使用SSL
    /// </summary>
    public bool Ssl { get; set; } = false;

    /// <summary>
    /// 连接失败时是否中止
    /// </summary>
    public bool AbortConnect { get; set; } = false;

    /// <summary>
    /// 连接超时时间（毫秒）
    /// </summary>
    public int ConnectTimeout { get; set; } = 5000;

    /// <summary>
    /// 同步超时时间（毫秒）
    /// </summary>
    public int SyncTimeout { get; set; } = 5000;

    /// <summary>
    /// 构建连接字符串
    /// </summary>
    /// <returns>Redis连接字符串</returns>
    public string BuildConnectionString()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append($"{Host}:{Port}");

        if (!string.IsNullOrEmpty(Password))
        {
            sb.Append($",password={Password}");
        }

        sb.Append($",defaultDatabase={Database}");
        sb.Append($",ssl={Ssl.ToString().ToLower()}");
        sb.Append($",abortConnect={AbortConnect.ToString().ToLower()}");
        sb.Append($",connectTimeout={ConnectTimeout}");
        sb.Append($",syncTimeout={SyncTimeout}");

        return sb.ToString();
    }
}