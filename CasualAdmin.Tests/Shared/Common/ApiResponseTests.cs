namespace CasualAdmin.Tests.Shared.Common
{
    using CasualAdmin.Shared.Common;
    using Xunit;

    /// <summary>
    /// API响应测试类
    /// </summary>
    public class ApiResponseTests
    {
        /// <summary>
        /// 测试成功响应
        /// </summary>
        [Fact]
        public void Success_ShouldReturnCorrectResponse()
        {
            // 调用Success方法
            var result = ApiResponse.Success("操作成功");

            // 断言响应格式正确
            Assert.NotNull(result);
            Assert.Equal(200, result.Code);
            Assert.Equal("操作成功", result.Message);
            Assert.True(result.Timestamp > 0);
        }

        /// <summary>
        /// 测试带数据的成功响应
        /// </summary>
        [Fact]
        public void Success_WithData_ShouldReturnCorrectResponse()
        {
            // 准备测试数据
            var testData = new { Name = "test", Value = 123 };

            // 调用带数据的Success方法
            var result = ApiResponse<object>.Success(testData, "操作成功");

            // 断言响应格式正确
            Assert.NotNull(result);
            Assert.Equal(200, result.Code);
            Assert.Equal("操作成功", result.Message);
            Assert.NotNull(result.Data);
            Assert.True(result.Timestamp > 0);
        }

        /// <summary>
        /// 测试失败响应
        /// </summary>
        [Fact]
        public void Failed_ShouldReturnCorrectResponse()
        {
            // 调用Failed方法
            var result = ApiResponse.Failed("操作失败");

            // 断言响应格式正确
            Assert.NotNull(result);
            Assert.Equal(500, result.Code);
            Assert.Equal("操作失败", result.Message);
            Assert.True(result.Timestamp > 0);
        }

        /// <summary>
        /// 测试带自定义错误码的失败响应
        /// </summary>
        [Fact]
        public void Failed_WithErrorCode_ShouldReturnCorrectResponse()
        {
            // 调用带自定义错误码的Failed方法
            var result = ApiResponse.Failed("参数错误", 400);

            // 断言响应格式正确
            Assert.NotNull(result);
            Assert.Equal(400, result.Code);
            Assert.Equal("参数错误", result.Message);
            Assert.True(result.Timestamp > 0);
        }

        /// <summary>
        /// 测试参数错误响应
        /// </summary>
        [Fact]
        public void BadRequest_ShouldReturnCorrectResponse()
        {
            // 调用BadRequest方法
            var result = ApiResponse.BadRequest("参数错误");

            // 断言响应格式正确
            Assert.NotNull(result);
            Assert.Equal(400, result.Code);
            Assert.Equal("参数错误", result.Message);
            Assert.True(result.Timestamp > 0);
        }

        /// <summary>
        /// 测试未授权响应
        /// </summary>
        [Fact]
        public void Unauthorized_ShouldReturnCorrectResponse()
        {
            // 调用Unauthorized方法
            var result = ApiResponse.Unauthorized("未授权");

            // 断言响应格式正确
            Assert.NotNull(result);
            Assert.Equal(401, result.Code);
            Assert.Equal("未授权", result.Message);
            Assert.True(result.Timestamp > 0);
        }

        /// <summary>
        /// 测试禁止访问响应
        /// </summary>
        [Fact]
        public void Forbidden_ShouldReturnCorrectResponse()
        {
            // 调用Forbidden方法
            var result = ApiResponse.Forbidden("禁止访问");

            // 断言响应格式正确
            Assert.NotNull(result);
            Assert.Equal(403, result.Code);
            Assert.Equal("禁止访问", result.Message);
            Assert.True(result.Timestamp > 0);
        }

        /// <summary>
        /// 测试资源不存在响应
        /// </summary>
        [Fact]
        public void NotFound_ShouldReturnCorrectResponse()
        {
            // 调用NotFound方法
            var result = ApiResponse.NotFound("资源不存在");

            // 断言响应格式正确
            Assert.NotNull(result);
            Assert.Equal(404, result.Code);
            Assert.Equal("资源不存在", result.Message);
            Assert.True(result.Timestamp > 0);
        }
    }
}