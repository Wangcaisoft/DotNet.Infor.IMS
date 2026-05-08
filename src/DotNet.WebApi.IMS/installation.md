# DotNet.WebApi.IMS - Installation

This document explains how to build and run the `DotNet.WebApi.IMS` Web API project.

Prerequisites

- Visual Studio (2017/2019/2022/2026) with .NET Framework development workload
- .NET Framework 4.6.2 Developer Pack (matches project target)
- IIS Express (installed with Visual Studio) or IIS

Build

1. Open the solution in Visual Studio.
2. Restore NuGet packages (Visual Studio will restore on build or use `Restore NuGet Packages`).
3. Build the solution.

Run in Visual Studio (IIS Express)

- Set `DotNet.WebApi.IMS` as the startup project.
- Press F5 to run with IIS Express.

Run in IIS

- Publish the project to a folder or to an IIS site using Visual Studio publish.
- Configure an IIS application or site and ensure the application pool targets the correct .NET Framework.

Configuration and folders

- The `ServiceController` writes decompressed payloads to `Data\MessagePayload` under the assembly parent folder. Ensure this folder exists and the App Pool identity has write permission.

HTTP endpoints

- `GET /ping`
- `GET /protocol`
- `POST /v3/multipartMessage` — see `README.md` for request requirements and example curl commands.

Notes

- The project includes the Help Page area for API documentation when running locally.
- For production: configure proper authentication, TLS certificates and environment-specific settings.
