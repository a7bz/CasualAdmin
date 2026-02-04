namespace CasualAdmin.Tests.Application.Services.System;

using CasualAdmin.Application.Interfaces.Events;
using CasualAdmin.Application.Interfaces.Services;
using CasualAdmin.Application.Services.System;
using CasualAdmin.Domain.Entities.System;
using CasualAdmin.Domain.Infrastructure.Data;
using Moq;
using Xunit;

/// <summary>
/// 部门服务测试类
/// </summary>
public class DeptServiceTests
{
    private readonly Mock<IRepository<SysDept>> _deptRepositoryMock;
    private readonly Mock<IValidationService> _validationServiceMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly DeptService _deptService;

    /// <summary>
    /// 测试类构造函数，初始化模拟对象和被测服务
    /// </summary>
    public DeptServiceTests()
    {
        // 初始化模拟对象
        _deptRepositoryMock = new Mock<IRepository<SysDept>>();
        _validationServiceMock = new Mock<IValidationService>();
        _eventBusMock = new Mock<IEventBus>();

        // 配置模拟验证服务
        _validationServiceMock.Setup(v => v.ValidateAndThrow(It.IsAny<SysDept>())).Verifiable();

        // 创建被测服务实例
        _deptService = new DeptService(_deptRepositoryMock.Object, _validationServiceMock.Object, _eventBusMock.Object);
    }

    /// <summary>
    /// 测试根据ID获取部门方法，当部门存在时返回正确的部门
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ShouldReturnDept_WhenDeptExists()
    {
        // Arrange
        var deptId = Guid.NewGuid();
        var expectedDept = new SysDept();

        expectedDept.SetDeptName("Test Dept");
        expectedDept.SetParent(null);
        expectedDept.SetSort(1);

        _deptRepositoryMock.Setup(r => r.GetByIdAsync(deptId)).ReturnsAsync(expectedDept);

        // Act
        var actualDept = await _deptService.GetByIdAsync(deptId);

        // Assert
        Assert.NotNull(actualDept);
        Assert.Equal(expectedDept.DeptId, actualDept.DeptId);
        Assert.Equal("Test Dept", actualDept.DeptName);
        Assert.Null(actualDept.ParentId);
        Assert.Equal(1, actualDept.Sort);
        _deptRepositoryMock.Verify(r => r.GetByIdAsync(deptId), Times.Once);
    }

    /// <summary>
    /// 测试获取所有部门方法，返回所有部门列表
    /// 验证返回的部门列表包含正确的部门信息
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDepts()
    {
        // Arrange
        var dept1 = new SysDept();
        dept1.SetDeptName("Dept 1");
        dept1.SetParent(null);
        dept1.SetSort(1);

        var dept2 = new SysDept();
        dept2.SetDeptName("Dept 2");
        dept2.SetParent(dept1.DeptId);
        dept2.SetSort(2);

        var depts = new List<SysDept> { dept1, dept2 };

        _deptRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(depts);

        // Act
        var result = await _deptService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(depts.Count, result.Count);
        Assert.Equal("Dept 1", result[0].DeptName);
        Assert.Equal("Dept 2", result[1].DeptName);
        _deptRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// 测试创建部门方法，确保能够创建部门并返回正确的部门信息
    /// 验证部门名称、父部门ID和排序字段是否正确设置
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldCreateDept()
    {
        // Arrange
        var dept = new SysDept();
        dept.SetDeptName("New Dept");
        dept.SetParent(null);
        dept.SetSort(1);

        var createdDept = new SysDept();
        createdDept.SetDeptName("New Dept");
        createdDept.SetParent(null);
        createdDept.SetSort(1);

        _deptRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SysDept>())).ReturnsAsync(createdDept);

        // Act
        var result = await _deptService.CreateAsync(dept);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdDept.DeptId, result.DeptId);
        Assert.Equal(createdDept.DeptName, result.DeptName);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.NotEqual(default, result.UpdatedAt);
        _deptRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SysDept>()), Times.Once);
    }

    /// <summary>
    /// 测试更新部门方法，当部门存在时返回成功结果
    /// 验证部门名称、父部门ID和排序字段是否按预期更新
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateDept_WhenDeptExists()
    {
        // Arrange
        var existingDept = new SysDept();
        existingDept.SetDeptName("Old Dept");
        existingDept.SetParent(null);
        existingDept.SetSort(1);

        var updatedDept = new SysDept();
        updatedDept.SetDeptName("Updated Dept");
        updatedDept.SetParent(null);
        updatedDept.SetSort(2);

        var savedDept = new SysDept();
        savedDept.SetDeptName("Updated Dept");
        savedDept.SetParent(null);
        savedDept.SetSort(2);

        _deptRepositoryMock.Setup(r => r.GetByIdAsync(existingDept.DeptId)).ReturnsAsync(existingDept);
        _deptRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<SysDept>())).ReturnsAsync(savedDept);

        // Act
        var result = await _deptService.UpdateAsync(updatedDept);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedDept.DeptId, result.DeptId);
        Assert.Equal("Updated Dept", result.DeptName);
        Assert.Equal(2, result.Sort);
        _deptRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SysDept>()), Times.Once);
    }

    /// <summary>
    /// 测试删除部门方法，当部门存在时返回成功结果
    /// 验证数据库操作是否按预期进行，包括事务提交
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenDeptExists()
    {
        // Arrange
        var deptId = Guid.NewGuid();
        _deptRepositoryMock.Setup(r => r.DeleteAsync(deptId)).ReturnsAsync(true);

        // Act
        var result = await _deptService.DeleteAsync(deptId);

        // Assert
        Assert.True(result);
        _deptRepositoryMock.Verify(r => r.DeleteAsync(deptId), Times.Once);
    }

    /// <summary>
    /// 测试删除部门方法，当部门不存在时返回失败结果
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenDeptDoesNotExist()
    {
        // Arrange
        var deptId = Guid.NewGuid();
        _deptRepositoryMock.Setup(r => r.DeleteAsync(deptId)).ReturnsAsync(false);

        // Act
        var result = await _deptService.DeleteAsync(deptId);

        // Assert
        Assert.False(result);
        _deptRepositoryMock.Verify(r => r.DeleteAsync(deptId), Times.Once);
    }

    /// <summary>
    /// 测试根据父部门ID获取子部门列表方法
    /// 验证能够正确返回指定父部门下的所有子部门
    /// 验证子部门列表是否按排序字段排序
    /// </summary>
    [Fact]
    public async Task GetChildrenByParentIdAsync_ShouldReturnChildrenDepts()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var child1 = new SysDept();
        child1.SetDeptName("Child 1");
        child1.SetParent(parentId);
        child1.SetSort(1);

        var child2 = new SysDept();
        child2.SetDeptName("Child 2");
        child2.SetParent(parentId);
        child2.SetSort(2);

        var children = new List<SysDept> { child1, child2 };
        _deptRepositoryMock.Setup(r => r.FindAsync(d => d.ParentId == parentId)).ReturnsAsync(children);

        // Act
        var result = await _deptService.GetChildrenByParentIdAsync(parentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(children.Count, result.Count);
        Assert.Equal("Child 1", result[0].DeptName);
        Assert.Equal("Child 2", result[1].DeptName);
        Assert.All(result, d => Assert.Equal(parentId, d.ParentId));
        _deptRepositoryMock.Verify(r => r.FindAsync(d => d.ParentId == parentId), Times.Once);
    }

    /// <summary>
    /// 测试获取部门树方法
    /// 验证能够正确返回部门树结构，包括根部门和所有子部门
    /// 验证部门树是否按排序字段排序
    /// </summary>
    [Fact]
    public async Task GetDeptTreeAsync_ShouldReturnDeptTree()
    {
        // Arrange
        var rootDept = new SysDept();
        rootDept.SetDeptName("Root Dept");
        rootDept.SetParent(null);
        rootDept.SetSort(1);

        var childDept1 = new SysDept();
        childDept1.SetDeptName("Child Dept 1");
        childDept1.SetParent(rootDept.DeptId);
        childDept1.SetSort(1);

        var childDept2 = new SysDept();
        childDept2.SetDeptName("Child Dept 2");
        childDept2.SetParent(rootDept.DeptId);
        childDept2.SetSort(2);

        var grandchildDept = new SysDept();
        grandchildDept.SetDeptName("Grandchild Dept");
        grandchildDept.SetParent(childDept1.DeptId);
        grandchildDept.SetSort(1);

        var allDepts = new List<SysDept> { rootDept, childDept1, childDept2, grandchildDept };
        _deptRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allDepts);

        // Act
        var result = await _deptService.GetDeptTreeAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result); // 只有一个根部门
        Assert.Equal(rootDept.DeptId, result[0].DeptId);
        Assert.Equal(2, result[0].Children?.Count); // 根部门有两个子部门
        Assert.Equal("Child Dept 1", result[0].Children?[0].DeptName);
        Assert.Equal("Child Dept 2", result[0].Children?[1].DeptName);
        Assert.Equal(1, result[0].Children?[0].Children?.Count); // 第一个子部门有一个子部门
    }
}
