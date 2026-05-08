namespace DotNet.IMS.Server.Models;

/// <summary>Ping 响应</summary>
public class PingResponse
{
    /// <summary>状态码</summary>
    public Int32 code { get; set; }

    /// <summary>消息</summary>
    public String message { get; set; }

    /// <summary>构造</summary>
    public PingResponse(Int32 code, String message)
    {
        this.code = code;
        this.message = message;
    }
}
