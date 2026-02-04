namespace CasualAdmin.Application.Profiles;

using AutoMapper;
using CasualAdmin.Application.Models.DTOs.Requests.System;
using CasualAdmin.Application.Models.DTOs.Responses.System;
using CasualAdmin.Domain.Entities.System;

/// <summary>
/// 系统模块映射配置
/// </summary>
public class SystemProfile : Profile
{
    /// <summary>
    /// 构造函数，配置映射规则
    /// </summary>
    public SystemProfile()
    {
        // 部门映射配置
        CreateMap<SysDept, SysDeptDto>();
        CreateMap<SysDept, SysDeptTreeDto>()
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children));
        CreateMap<SysDeptCreateDto, SysDept>();
        CreateMap<SysDeptUpdateDto, SysDept>();

        // 用户映射配置
        CreateMap<SysUser, SysUserDto>();
        CreateMap<SysUserCreateDto, SysUser>()
            .AfterMap((src, dest) =>
            {
                // 密码加密处理
                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<SysUser>();
                var salt = Guid.NewGuid().ToString();
                var hashedPassword = passwordHasher.HashPassword(dest, src.Password);
                dest.SetPassword(hashedPassword, salt);
            });
        CreateMap<SysUserUpdateDto, SysUser>()
            .AfterMap((src, dest) =>
            {
                // 如果提供了新密码，则重新加密
                if (!string.IsNullOrEmpty(src.Password))
                {
                    var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<SysUser>();
                    var salt = Guid.NewGuid().ToString();
                    var hashedPassword = passwordHasher.HashPassword(dest, src.Password);
                    dest.SetPassword(hashedPassword, salt);
                }
            });

        // 字典映射配置
        CreateMap<SysDict, SysDictDto>();
        CreateMap<SysDictCreateDto, SysDict>();
        CreateMap<SysDictUpdateDto, SysDict>();

        // 字典项映射配置
        CreateMap<SysDictItem, SysDictItemDto>();
        CreateMap<SysDictItemCreateDto, SysDictItem>();
        CreateMap<SysDictItemUpdateDto, SysDictItem>();

        // 角色映射配置
        CreateMap<SysRole, SysRoleDto>();
        CreateMap<SysRoleCreateDto, SysRole>();
        CreateMap<SysRoleUpdateDto, SysRole>();

        // 权限映射配置
        CreateMap<SysPermission, SysPermissionDto>();
        CreateMap<SysPermissionCreateDto, SysPermission>();
        CreateMap<SysPermissionUpdateDto, SysPermission>();

        // 菜单映射配置
        CreateMap<SysMenu, SysMenuDto>();
        CreateMap<SysMenu, SysMenuTreeDto>()
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children));
        CreateMap<SysMenuCreateDto, SysMenu>();
        CreateMap<SysMenuUpdateDto, SysMenu>();
    }
}
