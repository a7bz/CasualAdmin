namespace CasualAdmin.Tests.Shared.Common
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using CasualAdmin.Shared.Common;
    using Microsoft.IdentityModel.Tokens;
    using Xunit;

    /// <summary>
    /// JWT令牌助手测试类
    /// </summary>
    public class JwtTokenHelperTests
    {
        private const string SecretKey = "ThisIsASecretKeyForTestingPurposes1234567890";

        /// <summary>
        /// 生成测试用的JWT令牌
        /// </summary>
        private string GenerateTestToken(TokenUserInfo userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Name, userInfo.Username)
            };

            // 添加角色声明
            claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "test-issuer",
                audience: "test-audience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 测试从令牌中获取用户ID
        /// </summary>
        [Fact]
        public void GetUserIdFromToken_ShouldReturnCorrectUserId()
        {
            // 准备测试数据
            var userId = Guid.NewGuid();
            var userInfo = new TokenUserInfo
            {
                UserId = userId,
                Email = "test@example.com",
                Username = "testuser",
                Roles = new List<string> { "Admin" }
            };

            // 生成令牌
            var token = GenerateTestToken(userInfo);

            // 从令牌中获取用户ID
            var result = JwtTokenHelper.GetUserIdFromToken(token);

            // 断言用户ID正确
            Assert.Equal(userId, result);
        }

        /// <summary>
        /// 测试从令牌中获取邮箱
        /// </summary>
        [Fact]
        public void GetEmailFromToken_ShouldReturnCorrectEmail()
        {
            // 准备测试数据
            var userInfo = new TokenUserInfo
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                Roles = new List<string> { "Admin" }
            };

            // 生成令牌
            var token = GenerateTestToken(userInfo);

            // 从令牌中获取邮箱
            var result = JwtTokenHelper.GetEmailFromToken(token);

            // 断言邮箱正确
            Assert.Equal(userInfo.Email, result);
        }

        /// <summary>
        /// 测试从令牌中获取用户名
        /// </summary>
        [Fact]
        public void GetUsernameFromToken_ShouldReturnCorrectUsername()
        {
            // 准备测试数据
            var userInfo = new TokenUserInfo
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                Roles = new List<string> { "Admin" }
            };

            // 生成令牌
            var token = GenerateTestToken(userInfo);

            // 从令牌中获取用户名
            var result = JwtTokenHelper.GetUsernameFromToken(token);

            // 断言用户名正确
            Assert.Equal(userInfo.Username, result);
        }

        /// <summary>
        /// 测试从令牌中获取角色列表
        /// </summary>
        [Fact]
        public void GetRolesFromToken_ShouldReturnCorrectRoles()
        {
            // 准备测试数据
            var roles = new List<string> { "Admin", "User" };
            var userInfo = new TokenUserInfo
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                Roles = roles
            };

            // 生成令牌
            var token = GenerateTestToken(userInfo);

            // 从令牌中获取角色列表
            var result = JwtTokenHelper.GetRolesFromToken(token);

            // 断言角色列表正确
            Assert.Equal(roles.Count, result.Count);
            Assert.All(roles, role => Assert.Contains(role, result));
        }

        /// <summary>
        /// 测试从令牌中获取用户信息
        /// </summary>
        [Fact]
        public void GetUserInfoFromToken_ShouldReturnCorrectUserInfo()
        {
            // 准备测试数据
            var userInfo = new TokenUserInfo
            {
                UserId = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                Roles = new List<string> { "Admin", "User" }
            };

            // 生成令牌
            var token = GenerateTestToken(userInfo);

            // 从令牌中获取用户信息
            var result = JwtTokenHelper.GetUserInfoFromToken(token);

            // 断言用户信息正确
            Assert.NotNull(result);
            Assert.Equal(userInfo.UserId, result.UserId);
            Assert.Equal(userInfo.Email, result.Email);
            Assert.Equal(userInfo.Username, result.Username);
            Assert.Equal(userInfo.Roles.Count, result.Roles.Count);
        }

        /// <summary>
        /// 测试从Authorization头中提取token
        /// </summary>
        [Fact]
        public void ExtractTokenFromHeader_ShouldReturnCorrectToken()
        {
            // 准备测试数据
            var token = "test-token-123";
            var authorizationHeader = $"Bearer {token}";

            // 从Authorization头中提取token
            var result = JwtTokenHelper.ExtractTokenFromHeader(authorizationHeader);

            // 断言token提取正确
            Assert.Equal(token, result);
        }
    }
}