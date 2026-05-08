using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DotNet.IMS.Server.Services;

public class TenantValidator : ITenantValidator
{
    private readonly String _validTenantRegex = "^[A-Z0-9]{1,17}_[A-Z0-9]{3}$";

    public IActionResult? Validate(HttpRequest request)
    {
        String? tenantId = null;
        if (request.Headers.TryGetValue("X-TenantId", out var vals)) tenantId = vals.FirstOrDefault();

        if (String.IsNullOrEmpty(tenantId))
            return new ObjectResult(new { status = "FAIL", code = 418, message = DotNet.IMS.Server.Constants.MSG_RESPONSE_418 }) { StatusCode = 412 };

        if (tenantId.Length > 22)
            return new ObjectResult(new { status = "FAIL", code = 448, message = DotNet.IMS.Server.Constants.MSG_RESPONSE_448 }) { StatusCode = 412 };

        var m = Regex.Match(tenantId, _validTenantRegex, RegexOptions.IgnoreCase);
        if (!m.Success && !tenantId.Equals("INFOR", StringComparison.InvariantCultureIgnoreCase))
            return new ObjectResult(new { status = "FAIL", code = 458, message = String.Format(DotNet.IMS.Server.Constants.MSG_RESPONSE_456, tenantId) }) { StatusCode = 412 };

        return null;
    }
}
