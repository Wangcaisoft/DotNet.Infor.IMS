# IMSSampleApplication.NET.8 安装与运行说明（中/英）

## 简体中文（Chinese）

本文档说明如何在本地构建、运行和验证 IMSSampleApplication.NET.8（基于 .NET 8）的示例服务。该项目是对原 Infor IMS 示例的 .NET 8 移植版，包含 `/ping`、`/protocol` 与 `/v3/multipartMessage` 接口。

先决条件
- 已安装 .NET 8 SDK（运行 `dotnet --version`，应为 8.x）
- PowerShell（Windows）或等效终端
- 可选：Docker（容器化时使用）

主要文件（相关）
- IMSSampleApplication.NET.8/IMSSampleApplication.NET.8.csproj
- IMSSampleApplication.NET.8/Program.cs
- IMSSampleApplication.NET.8/Controllers/ServiceController.cs
- IMSSampleApplication.NET.8/Models/*.cs
- IMSSampleApplication.NET.8/Helpers/CompressionHelper.cs
- 应用运行时写入位置：`<app_base>/Data/MessagePayload`

本地构建与运行（PowerShell）
1. 切换到项目目录：

   cd D:\GitHub\DotNet.Infor.IMS\src\IMSSampleApplication.NET.8

2. 恢复并构建：

   dotnet restore
   dotnet build --configuration Debug

3. 启动服务：

   dotnet run

   控制台将显示监听的 URL（例如 http://localhost:5000 或 https://localhost:5001）。

4. 验证接口示例：

   - Ping:
     curl http://localhost:{port}/ping
     返回示例：
     { "code": 200, "message": "OK" }

   - Protocol:
     curl http://localhost:{port}/protocol

   - Multipart 上传示例（curl）：

     curl -X POST "http://localhost:{port}/v3/multipartMessage" \
       -H "X-TenantId: INFOR" \
       -F "ParameterRequest=@parameter.json;type=application/json" \
       -F "MessagePayload=@payload.bin;type=application/octet-stream"

     说明：`payload.bin` 应为 zlib/deflate 封装的压缩数据，服务会检查前 3 字节签名并尝试解压。

Swagger / OpenAPI
- 开发环境已启用 Swagger，可访问：
  http://localhost:{port}/swagger

运行时注意
- 解压后的有效载荷写入运行目录下的 `Data/MessagePayload` 文件。请确保运行用户对该目录有写权限。
- 接口返回保持与原示例兼容（字段：status/code/message），错误码使用定义的常量。

调试与排错
- 若遇构建错误，先运行 `dotnet build -v minimal` 获取详细信息。
- 若有 NuGet 兼容或安全警告（来自旧包），请列出受影响包以便统一评估和升级。

后续建议
- 将写入路径、请求体限制、日志级别等配置化（appsettings.json）。
- 为关键逻辑（multipart、压缩、tenant 验证）添加单元与集成测试。
- 需要容器化或 CI/CD 时可添加 Dockerfile 与 GitHub Actions 工作流。

---

## English

This document explains how to build, run and validate IMSSampleApplication.NET.8 (a .NET 8 sample server). It is a port of the original Infor IMS sample and exposes `/ping`, `/protocol` and `/v3/multipartMessage` endpoints.

Prerequisites
- .NET 8 SDK installed (`dotnet --version` should report 8.x)
- PowerShell (Windows) or equivalent shell
- Optional: Docker for containerized deployments

Key files
- IMSSampleApplication.NET.8/IMSSampleApplication.NET.8.csproj
- IMSSampleApplication.NET.8/Program.cs
- IMSSampleApplication.NET.8/Controllers/ServiceController.cs
- IMSSampleApplication.NET.8/Models/*.cs
- IMSSampleApplication.NET.8/Helpers/CompressionHelper.cs
- Runtime output path: `<app_base>/Data/MessagePayload`

Build and run locally (PowerShell)
1. Change to project directory:

   cd D:\GitHub\DotNet.Infor.IMS\src\IMSSampleApplication.NET.8

2. Restore and build:

   dotnet restore
   dotnet build --configuration Debug

3. Run the app:

   dotnet run

   The console shows the listening URL(s) (for example http://localhost:5000).

4. Verify endpoints:

   - Ping:
     curl http://localhost:{port}/ping
     Example response:
     { "code": 200, "message": "OK" }

   - Protocol:
     curl http://localhost:{port}/protocol

   - Multipart example (curl):

     curl -X POST "http://localhost:{port}/v3/multipartMessage" \
       -H "X-TenantId: INFOR" \
       -F "ParameterRequest=@parameter.json;type=application/json" \
       -F "MessagePayload=@payload.bin;type=application/octet-stream"

     Note: `payload.bin` should contain zlib/deflate-wrapped compressed data. The service checks the first bytes signature and attempts to decompress.

Swagger / OpenAPI
- Swagger UI is available in development: http://localhost:{port}/swagger

Runtime notes
- Decompressed payload is written to `Data/MessagePayload` under the application's base directory. Ensure the process has write permission.
- Responses follow the sample structure (status/code/message). Error codes are defined in constants.

Troubleshooting
- For build errors run `dotnet build -v minimal` for details.
- If you see NuGet compatibility or security warnings related to older packages, list the affected packages for a coordinated upgrade.

Next steps (recommendations)
- Externalize configuration (write path, max request size, logging) to `appsettings.json`.
- Add unit and integration tests for multipart handling, compression and tenant validation.
- If you need containerization or CI, I can add a Dockerfile and GitHub Actions workflow.

(End)
