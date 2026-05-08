# Infor IMS Sample Applications

This repository contains two .NET Framework Web API sample projects implementing an IMS-style multipart message service and related helpers.

- `Infor.IMSSampleApplication.Server` (self-host sample) — sample HTTP self-host server and helpers (targets .NET Framework 4.6.1)
- `DotNet.WebApi.IMS` (IIS / Web API sample) — Web API project with HelpPage and controllers (targets .NET Framework 4.6.2)

License: Apache License 2.0 (see source file headers).

## Overview

Both projects implement the same core HTTP endpoints used by IMS clients:

- `GET /ping` — lightweight health check. Returns HTTP 200 with a JSON body `{"code":200,"status":"OK"}`.
- `GET /protocol` — returns supported protocol metadata in JSON (version, message method, encoding, charset, discovery flag).
- `POST /v3/multipartMessage` — primary endpoint for multipart messages containing exactly two parts:
  - a `ParameterRequest` part (JSON, `application/json`)
  - a `MessagePayload` part (binary, `application/octet-stream`)

## Multipart request rules

The server enforces the following rules for `POST /v3/multipartMessage`:

- The multipart body must contain exactly two parameters (keys): `ParameterRequest` and `MessagePayload`.
- Each of those may be provided as form fields or as file parts. Both must be present.
- `ParameterRequest` must be `application/json`. It must not be empty.
- `MessagePayload` must be `application/octet-stream` and contain the compressed payload.
- Requests must include the `X-TenantId` header. Validation rules:
  - Header must be present and non-empty.
  - Maximum length 22 characters.
  - Valid format (case-insensitive): `^[A-Z0-9]{1,17}_[A-Z0-9]{3}$` or the literal tenant id `INFOR` is accepted.

If the request violates the rules, the server responds with HTTP 412 (Precondition Failed) and a JSON `MultipartMessageResponse` containing a numeric `code`, `status` and `message`. Common response messages are defined in `Models/Constants.cs`.

## Compression format

- The expected payload encoding is DEFLATE. The helper `Helpers/CompressionHelper.cs` detects common zlib/deflate signatures and uses a `DeflateStream` to decompress.
- Implementation notes: the decompressor code reads and discards the first two bytes (zlib header) and then passes the remainder to `DeflateStream`. When preparing payloads, produce data encoded with zlib-wrapped deflate (typical zlib-compressed output) so it matches the detection and decompression behavior.

## Where the request payload is written

- When a valid `MessagePayload` is received it is decompressed and written to a path under the project `Data\MessagePayload` (relative to the assembly location). See `ServiceController.processMessagePayload`.

## Running and building

Open the solution in Visual Studio (projects target .NET Framework 4.6.1 / 4.6.2). Build as usual.

- To run the `DotNet.WebApi.IMS` project, start it with IIS Express or a full IIS site inside Visual Studio.
- The self-host sample `Infor.IMSSampleApplication.Server` exposes a `SelfHostStarter.Start(baseAddress, formatterSettings)` helper to start an `HttpSelfHostServer` if you want to host from a console app or service.

## Example curl (multipart) request

Replace `https://localhost:44300` with your server address.

curl example using file parts:

`curl -k -X POST "https://localhost:44300/v3/multipartMessage" -H "X-TenantId: ACME_001" -F "ParameterRequest=@param.json;type=application/json" -F "MessagePayload=@payload.bin;type=application/octet-stream"`

- `param.json` is the JSON parameter request content.
- `payload.bin` is the zlib-wrapped deflate compressed binary payload.

Note: `-k` disables TLS verification for local testing. For production use a valid TLS certificate.

## Useful files and classes

- `Controllers/ServiceController.cs` — endpoint implementations and request validation.
- `Helpers/CompressionHelper.cs` — DEFLATE detection and decompression utility.
- `Models/Constants.cs` — standard response messages and codes used by the service.
- `SelfHostStarter` (in the self-host project) — helper to start a self-hosted server instance.

## Contributing

This is a sample/demo repository. If you plan to modify behavior for production workloads, ensure you add proper logging, authentication, stronger tenant validation and robust error handling.

## Contact / Support

This project is an example implementation. For questions about using it in your environment, inspect the `ServiceController` and `CompressionHelper` implementations and adapt to your deployment and security requirements.
