# DotNet.IMS.Server 安装与运行说明（中/英）

## 简体中文（Chinese）

本文档说明如何在本地构建、运行和验证 DotNet.IMS.Server（基于 .NET 8）的示例服务。该项目实现了 Infor IMS (ION Message Service) v3 的示例 Server 端，包含 `/ping`、`/protocol` 与 `/v3/multipartMessage` 接口。

先决条件
- 已安装 .NET 8 SDK（运行 `dotnet --version`，应为 8.x）
- PowerShell（Windows）或等效终端
- 可选：Docker（容器化时使用）

主要文件
- DotNet.IMS.Server/DotNet.IMS.Server.csproj
- DotNet.IMS.Server/Program.cs
- DotNet.IMS.Server/Controllers/ServiceController.cs
- DotNet.IMS.Server/Models/*.cs
- DotNet.IMS.Server/Helpers/CompressionHelper.cs
- 运行时写入位置：`<app_base>/Data/MessagePayload`

本地构建与运行（PowerShell）
1. 切换到项目目录：

   cd D:\GitHub\DotNet.Infor.IMS\src\DotNet.IMS.Server

2. 恢复并构建：

   dotnet restore
   dotnet build --configuration Debug

3. 启动服务：

   dotnet run

   控制台将显示监听的 URL（例如 http://localhost:5000）。

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
- 解压后的有效载荷写入应用基目录下的 `Data/MessagePayload` 文件。请确保运行用户对该目录有写权限。
- 接口返回遵循示例结构（status/code/message），错误码在 Constants 中定义。

调试与排错
- 若遇构建错误，运行 `dotnet build -v minimal` 获取详细信息。
- 若看到 NuGet 兼容或安全警告（来自旧包），请列出受影响包以便统一评估升级方案。

后续建议
- 将写入路径、请求体限制、日志级别等配置化为 appsettings.json。
- 为 multipart 处理、压缩解压、tenant 验证添加单元与集成测试。
- 如需容器化或 CI/CD，我可以添加 Dockerfile 与 GitHub Actions 工作流。

---

## English

This document explains how to build, run and validate DotNet.IMS.Server (a .NET 8 sample server implementing Infor IMS v3).

Prerequisites
- .NET 8 SDK installed (`dotnet --version` should report 8.x)
- PowerShell or equivalent shell
- Optional: Docker for containerized deployments

Key files
- DotNet.IMS.Server/DotNet.IMS.Server.csproj
- DotNet.IMS.Server/Program.cs
- DotNet.IMS.Server/Controllers/ServiceController.cs
- DotNet.IMS.Server/Models/*.cs
- DotNet.IMS.Server/Helpers/CompressionHelper.cs
- Runtime output path: `<app_base>/Data/MessagePayload`

Build and run locally (PowerShell)
1. Change to project directory:

   cd D:\GitHub\DotNet.Infor.IMS\src\DotNet.IMS.Server

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

     Note: `payload.bin` should contain zlib/deflate-wrapped compressed data. The service checks the leading signature bytes and attempts to decompress.

Swagger / OpenAPI
- Swagger UI is available in development: http://localhost:{port}/swagger

Runtime notes
- Decompressed payload is written to `Data/MessagePayload` under the application's base directory. Ensure the process has write permission.
- Responses follow sample structure (status/code/message). Error codes are defined in Constants.

Troubleshooting
- For build errors run `dotnet build -v minimal` for details.
- If you see NuGet compatibility or security warnings related to older packages, list the affected packages for coordinated upgrade.

Next steps (recommendations)
- Externalize configuration (write path, max request size, logging) to `appsettings.json`.
- Add unit and integration tests for multipart handling, compression and tenant validation.
- If you need containerization or CI, I can add a Dockerfile and GitHub Actions workflow.

(End)
