namespace CasualAdmin.Shared.Common;

/// <summary>
/// 分页响应结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class PageResponse<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevious => PageIndex > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNext => PageIndex < TotalPages;

    /// <summary>
    /// 构造函数
    /// </summary>
    public PageResponse()
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="items">数据列表</param>
    /// <param name="total">总记录数</param>
    /// <param name="pageIndex">当前页码</param>
    /// <param name="pageSize">每页数量</param>
    public PageResponse(List<T> items, int total, int pageIndex, int pageSize)
    {
        Items = items;
        Total = total;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    /// <summary>
    /// 创建分页响应
    /// </summary>
    /// <param name="items">数据列表</param>
    /// <param name="total">总记录数</param>
    /// <param name="pageIndex">当前页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>分页响应</returns>
    public static PageResponse<T> Create(List<T> items, int total, int pageIndex, int pageSize)
    {
        return new PageResponse<T>(items, total, pageIndex, pageSize);
    }
}