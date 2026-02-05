namespace CasualAdmin.Shared.Common;

/// <summary>
/// 分页请求基类
/// </summary>
public class PageRequest
{
    private int _pageIndex = 1;
    private int _pageSize = 10;

    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value < 1 ? 1 : value;
    }

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向（asc/desc）
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// 计算跳过的记录数
    /// </summary>
    public int Skip => (PageIndex - 1) * PageSize;
}

/// <summary>
/// 分页请求基类（带筛选器）
/// </summary>
/// <typeparam name="TFilter">筛选器类型</typeparam>
public class PageRequest<TFilter> : PageRequest where TFilter : class, new()
{
    /// <summary>
    /// 筛选条件对象
    /// </summary>
    public TFilter Filter { get; set; } = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    public PageRequest()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="filter">筛选条件对象</param>
    public PageRequest(TFilter filter)
    {
        Filter = filter;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="filter">筛选条件对象</param>
    public PageRequest(int pageIndex, int pageSize, TFilter filter)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Filter = filter;
    }
}