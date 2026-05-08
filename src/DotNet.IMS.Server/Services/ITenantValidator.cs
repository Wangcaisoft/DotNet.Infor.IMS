namespace DotNet.IMS.Server.Services;

public interface ITenantValidator
{
    /// <summary>验证租户ID，返回错误响应信息或 null 表示通过</summary>
    Microsoft.AspNetCore.Mvc.IActionResult? Validate(Microsoft.AspNetCore.Http.HttpRequest request);
}
