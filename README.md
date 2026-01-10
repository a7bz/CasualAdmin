# CasualAdmin

CasualAdmin 是一个基于 .NET 8.0 开发的轻量级后台管理系统框架，提供了用户管理、角色管理、权限控制等核心功能，采用了分层架构设计，便于扩展和维护。

## 技术栈

- **框架**: .NET 8.0
- **Web API**: ASP.NET Core Web API
- **ORM**: SqlSugarCore
- **认证授权**: JWT (JSON Web Token)
- **API文档**: Swagger + Knife4j UI
- **依赖注入**: Microsoft.Extensions.DependencyInjection
- **日志**: Serilog
- **数据验证**: FluentValidation
- **对象映射**: AutoMapper

## 系统架构

CasualAdmin 采用了经典的分层架构设计，各层职责明确，便于扩展和维护：

- **API层**: 处理HTTP请求，负责路由、参数验证和响应格式化
- **Application层**: 实现业务逻辑，协调各服务之间的交互
- **Domain层**: 定义核心业务模型和领域规则
- **Infrastructure层**: 提供数据访问、文件存储、第三方服务集成等基础设施支持
- **Shared层**: 包含共享的常量、工具类、扩展方法等

## 安装步骤

### 1. 克隆项目

```bash
git clone https://github.com/your-username/CasualAdmin.git
cd CasualAdmin
```

### 2. 安装依赖

```bash
dotnet restore
```

### 3. 配置数据库

修改 `CasualAdmin.API/appsettings.json` 文件中的数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CasualAdmin;Username=postgres;Password=123456;Pooling=true"
  },
  "Database": {
    "Type": "PostgreSQL" // 支持 PostgreSQL、SQL Server、MySQL、SQLite 等
  }
}
```

### 4. 数据库迁移

项目使用 SqlSugar ORM，支持自动创建表结构。首次运行项目时，会自动根据实体类创建数据库表。

## 运行项目

### 开发环境

```bash
dotnet run --project CasualAdmin.API
```

或者在 Visual Studio 中直接运行 `CasualAdmin.API` 项目。

### 生产环境

```bash
dotnet publish -c Release -o publish
cd publish
dotnet CasualAdmin.API.dll
```

## API文档

项目集成了 Swagger 和 Knife4j UI，提供了友好的 API 文档界面：

- **Knife4j UI**: http://localhost:5001/swagger

## 日志配置

### Serilog 配置

项目使用 Serilog 进行日志记录，配置位于 `appsettings.json` 文件中的 `Serilog` 节点：

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

### 日志中间件配置

项目包含自定义的 `LoggingMiddleware`，用于记录请求和响应信息，配置位于 `appsettings.json` 文件中的 `LoggingMiddleware` 节点：

```json
{
  "LoggingMiddleware": {
    "IncludeRequestBody": true,
    "IncludeResponseBody": false,
    "RequestBodySizeLimit": 102400,
    "ResponseBodySizeLimit": 1048576,
    "ExcludePaths": [
      "/swagger",
      "/v1/swagger.json"
    ],
    "ExcludeMethods": []
  }
}
```

#### 配置说明：

- `IncludeRequestBody`: 是否记录请求体
- `IncludeResponseBody`: 是否记录响应体
- `RequestBodySizeLimit`: 请求体大小限制（字节）
- `ResponseBodySizeLimit`: 响应体大小限制（字节）
- `ExcludePaths`: 排除记录日志的路径
- `ExcludeMethods`: 排除记录日志的HTTP方法

## 项目结构

```
CasualAdmin/
├── CasualAdmin.API/            # API层
│   ├── Controllers/           # API控制器
│   ├── Middleware/             # 中间件
│   ├── Properties/            # 项目属性
│   ├── CasualAdmin.API.csproj
│   ├── Program.cs             # 项目入口
│   └── appsettings.json       # 配置文件
├── CasualAdmin.Application/   # 应用层
│   ├── Commands/              # 命令定义
│   ├── Interfaces/            # 服务接口
│   ├── Profiles/              # AutoMapper配置
│   ├── Services/              # 服务实现
│   ├── Validators/            # FluentValidation验证器
│   └── CasualAdmin.Application.csproj
├── CasualAdmin.Domain/        # 领域层
│   ├── Entities/              # 实体类
│   └── CasualAdmin.Domain.csproj
├── CasualAdmin.Infrastructure/ # 基础设施层
│   ├── Data/                  # 数据访问
│   ├── Services/              # 基础设施服务
│   └── CasualAdmin.Infrastructure.csproj
├── CasualAdmin.Shared/        # 共享层
│   ├── Common/                # 公共类
│   └── CasualAdmin.Shared.csproj
└── CasualAdmin.sln            # 解决方案文件
```

## 功能模块

### 1. 用户管理

- 用户注册
- 用户登录
- 用户查询
- 用户创建、更新、删除
- 密码加密存储

### 2. 角色管理

- 角色查询
- 角色创建、更新、删除
- 为用户分配角色
- 从用户移除角色

### 3. 认证授权

- JWT Token生成
- JWT Token验证
- 基于角色的访问控制

## 开发指南

### 1. 添加新功能

1. 在 `Domain` 层定义实体类
2. 在 `Application` 层定义服务接口和实现
3. 在 `Infrastructure` 层实现数据访问
4. 在 `API` 层添加控制器和路由

### 2. 添加新的API端点

1. 在 `Controllers` 目录下创建或修改控制器
2. 添加API方法，使用 `[HttpGet]`、`[HttpPost]` 等属性指定HTTP方法
3. 使用 `[Route]` 属性指定路由模板
4. 在方法参数中使用 `[FromBody]`、`[FromQuery]` 等属性指定参数来源

### 3. 添加数据验证

1. 在 `Validators` 目录下创建验证器类
2. 继承 `AbstractValidator<T>`，其中 `T` 是要验证的模型类型
3. 在构造函数中使用 `RuleFor` 方法定义验证规则

### 4. 添加对象映射

1. 在 `Profiles` 目录下创建或修改 Profile 类
2. 继承 `Profile` 类
3. 在构造函数中使用 `CreateMap` 方法定义映射规则

## 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

## 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 联系方式

- 项目地址: https://github.com/a7bz/CasualAdmin
- 问题反馈: https://github.com/a7bz/CasualAdmin/issues