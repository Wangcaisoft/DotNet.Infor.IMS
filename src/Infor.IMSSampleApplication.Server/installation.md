# Infor.IMSSampleApplication.Server - Installation

This document explains how to build and run the self-host sample server in `Infor.IMSSampleApplication.Server` and includes helper scripts, a sample parameter JSON, and a console host project for easy local testing.

## Prerequisites

- Visual Studio (2017/2019/2022/2026) with .NET Framework development workload
- .NET Framework 4.6.1 Developer Pack (matches project target)
- NuGet package restore (Visual Studio will restore on build)

## Build

1. Open the solution in Visual Studio.
2. Restore NuGet packages (Visual Studio does this automatically on build or use `Restore NuGet Packages`).
3. Build the solution.

## Run options

### Option A — Run in IIS / IIS Express

- If the project is configured as a Web project (contains `Global.asax`), set `Infor.IMSSampleApplication.Server` as the startup project and press F5 (IIS Express) or configure a site in IIS and run.

### Option B — Self-host from a console or service

- The project includes a helper `SelfHostStarter.Start(baseAddress, formatterSettings)` to start an `HttpSelfHostServer`.
- A ready-made console host project is included under `ConsoleHost/` that references this project. To run it:

  1. Right-click the `ConsoleHost` project in Solution Explorer and choose `Set as Startup Project`.
  2. Ensure `ConsoleHost.csproj` references the `Infor.IMSSampleApplication.Server` project and has `Microsoft.AspNet.WebApi.SelfHost` package available (the provided `ConsoleHost.csproj` already includes these references).
  3. Run the console host. By default it will start at `https://localhost:44300`. You may pass a different base address as a command-line argument.

- For HTTPS (transport security) the helper uses `MyHttpsSelfHostConfiguration`. Ensure a valid certificate is bound to the host/port or use local test certificates.

## File system / permissions

- The `ServiceController` writes decompressed payloads to `Data\\MessagePayload` located under the assembly parent folder. Create the `Data\\MessagePayload` folder or ensure the process has write permissions to that path.
- A helper PowerShell script `create-data.ps1` is included to create the folder and set write ACLs for the current user. Run from the `Infor.IMSSampleApplication.Server` folder:

  ```powershell
  .\\create-data.ps1
  ```

## Sample parameter JSON and payload generation

- A sample `param.json` is provided at `sample/param.json`.
- To create a compressed payload usable by the service you need zlib-wrapped deflate bytes (the `CompressionHelper` detects common zlib headers). Example PowerShell snippet to compress a file using a simple zlib wrapper is not included by default because .NET's `DeflateStream` writes raw deflate data without the zlib header. Below is a PowerShell example that uses a small helper to add a zlib header around a deflate stream output (for local testing only):

  ```powershell
  # Compress file to raw deflate
  $input = [System.IO.File]::ReadAllBytes("sample\\param.json")
  $ms = New-Object System.IO.MemoryStream
  $ds = New-Object System.IO.Compression.DeflateStream($ms, [System.IO.Compression.CompressionMode]::Compress)
  $ds.Write($input, 0, $input.Length)
  $ds.Close()
  $rawDeflate = $ms.ToArray()

  # Prepend a simple zlib header (0x78 0x9C is common) and append Adler32 checksum if needed for strict zlib format.
  [byte[]]$zlibHeader = 0x78,0x9C
  [byte[]]$payload = New-Object byte[] ($zlibHeader.Length + $rawDeflate.Length)
  [Array]::Copy($zlibHeader, 0, $payload, 0, $zlibHeader.Length)
  [Array]::Copy($rawDeflate, 0, $payload, $zlibHeader.Length, $rawDeflate.Length)

  [System.IO.File]::WriteAllBytes("sample\\payload.bin", $payload)
  ```

  - The above creates a payload that should be recognized by `CompressionHelper.isDataInDeflatedFormat` for basic testing. For production payloads use proper zlib-producing libraries or tools.

## Notes

- The service expects incoming multipart requests as described in the repository `README.md`.
- For production use: add robust logging, secure certificate setup and appropriate access controls.

## Troubleshooting

- If the server cannot bind HTTPS ports, check certificate bindings and run Visual Studio elevated if necessary for port reservations.
