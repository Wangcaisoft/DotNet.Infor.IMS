namespace DotNet.IMS.Server.Models;

/// <summary>协议响应</summary>
public class ProtocolResponse
{
    public String version { get; set; }
    public String messageMethod { get; set; }
    public String supportedEncoding { get; set; }
    public String supportedCharacterSet { get; set; }
    public Boolean hasDiscovery { get; set; }

    public ProtocolResponse(String version, String messageMethod, String supportedEncoding, String supportedCharacterSet, Boolean hasDiscovery)
    {
        this.version = version;
        this.messageMethod = messageMethod;
        this.supportedEncoding = supportedEncoding;
        this.supportedCharacterSet = supportedCharacterSet;
        this.hasDiscovery = hasDiscovery;
    }
}
