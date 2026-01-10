namespace CasualAdmin.Application.Models.DTOs.Responses.System;

/// <summary>
/// 部门树DTO
/// </summary>
public class SysDeptTreeDto : SysDeptDto
{
    /// <summary>
    /// 子部门列表
    /// </summary>
    public List<SysDeptTreeDto> Children { get; set; } = [];
}