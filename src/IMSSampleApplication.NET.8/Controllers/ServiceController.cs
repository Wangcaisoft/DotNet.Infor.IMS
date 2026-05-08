using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using IMSSampleApplication.Helpers;
using IMSSampleApplication.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

namespace IMSSampleApplication.Net8.Controllers
{
    [ApiController]
    [Route("/")]
    public class ServiceController : ControllerBase
    {
        private string validTenantRegex = "^[A-Z0-9]{1,17}_[A-Z0-9]{3}$";

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            var pingResponse = new PingResponse(200, "OK");
            return Ok(pingResponse);
        }

        [HttpGet("protocol")]
        public IActionResult Protocol()
        {
            var protocolResponse = new ProtocolResponse("v3", "multipartMessage", "DEFLATE", "UTF-8", false);
            return Ok(protocolResponse);
        }

        [HttpPost("v3/multipartMessage")]
        public async Task<IActionResult> MultipartMessage()
        {
            var form = await Request.ReadFormAsync();
            if (form == null)
            {
                return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 477, Constants.MSG_RESPONSE_477);
            }

            // collect all keys (fields and file names)
            var keys = new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            foreach (var k in form!.Keys) keys.Add(k);
            foreach (var f in form!.Files) keys.Add(f.Name);

            if (keys.Count != 2)
            {
                return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 477, Constants.MSG_RESPONSE_477);
            }

            var validateTenantResponse = ValidateTenantId();
            if (validateTenantResponse != null) return validateTenantResponse;

            bool parameterRequestFound = false;
            bool messagePayloadFound = false;

            // process form fields
            if (form != null)
            {
                if (form.TryGetValue("ParameterRequest", out var paramVals))
                {
                    var parameterRequest = paramVals.ToString();
                    if (string.IsNullOrEmpty(parameterRequest))
                    {
                        return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 453, Constants.MSG_RESPONSE_453);
                    }
                    parameterRequestFound = true;
                }
                if (form.TryGetValue("MessagePayload", out var payloadVals))
                {
                    var payloadString = payloadVals.ToString();
                    var payloadContent = Encoding.UTF8.GetBytes(payloadString);
                    var response = ProcessMessagePayload(payloadContent);
                    if (response != null) return response;
                    messagePayloadFound = true;
                }
            }

            // process files
            foreach (var file in form!.Files)
            {
                if (!"ParameterRequest".Equals(file.Name, System.StringComparison.OrdinalIgnoreCase) && !"MessagePayload".Equals(file.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 477, Constants.MSG_RESPONSE_477);
                }
                if ("ParameterRequest".Equals(file.Name, System.StringComparison.OrdinalIgnoreCase) && !"application/json".Equals(file.ContentType, System.StringComparison.OrdinalIgnoreCase))
                {
                    return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 470, Constants.MSG_RESPONSE_470_PARAMETER_INVALIDTYPE);
                }
                else if ("MessagePayload".Equals(file.Name, System.StringComparison.OrdinalIgnoreCase) && !"application/octet-stream".Equals(file.ContentType, System.StringComparison.OrdinalIgnoreCase))
                {
                    return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 470, Constants.MSG_RESPONSE_470_PAYLOAD_INVALIDTYPE);
                }

                if ("ParameterRequest".Equals(file.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var parameterContent = ms.ToArray();
                    if (parameterContent.Length == 0)
                    {
                        return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 453, Constants.MSG_RESPONSE_453);
                    }
                    parameterRequestFound = true;
                }
                if ("MessagePayload".Equals(file.Name, System.StringComparison.OrdinalIgnoreCase))
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var payloadContent = ms.ToArray();
                    var response = ProcessMessagePayload(payloadContent);
                    messagePayloadFound = true;
                    if (response != null) return response;
                }
            }

            if (!parameterRequestFound || !messagePayloadFound)
            {
                return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 477, Constants.MSG_RESPONSE_477);
            }

            return BuildMessageResponse("OK", HttpStatusCode.OK, 202, Constants.MSG_RESPONSE_202);
        }

        private IActionResult? ProcessMessagePayload(byte[] payloadContent)
        {
            var baseDir = AppContext.BaseDirectory;
            var writeDir = Path.Combine(baseDir, "Data");
            Directory.CreateDirectory(writeDir);
            var writeLocation = Path.Combine(writeDir, "MessagePayload");

            if (!CompressionHelper.isDataInDeflatedFormat(payloadContent))
            {
                return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 450, Constants.MSG_RESPONSE_450);
            }
            else
            {
                var payloadContentInflated = CompressionHelper.Decompress(payloadContent);
                System.IO.File.WriteAllBytes(writeLocation, payloadContentInflated);
            }
            return null;
        }

        private IActionResult BuildMessageResponse(string statusEnum, HttpStatusCode httpStatus, int code, string msg)
        {
            var messageResponse = new MultipartMessageResponse(statusEnum, code, msg);
            return StatusCode((int)httpStatus, messageResponse);
        }

        private IActionResult? ValidateTenantId()
        {
            string? tenantId = null;
            if (Request.Headers.TryGetValue("X-TenantId", out var vals)) tenantId = vals.FirstOrDefault();

            if (string.IsNullOrEmpty(tenantId))
            {
                return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 418, Constants.MSG_RESPONSE_418);
            }
            if (tenantId.Length > 22)
            {
                return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 448, Constants.MSG_RESPONSE_448);
            }
            Match m = Regex.Match(tenantId, validTenantRegex, RegexOptions.IgnoreCase);
            if (!m.Success && !tenantId.Equals("INFOR", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return BuildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 458, string.Format(Constants.MSG_RESPONSE_456, tenantId));
            }
            return null;
        }
    }
}
