namespace DotNet.IMS.Server.Models;

/// <summary>多部分消息响应</summary>
public class MultipartMessageResponse
{
    public String status { get; set; }
    public Int32 code { get; set; }
    public String message { get; set; }

    public MultipartMessageResponse(String status, Int32 code, String message)
    {
        this.status = status;
        this.code = code;
        this.message = message;
    }
}
