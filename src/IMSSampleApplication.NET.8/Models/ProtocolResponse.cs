namespace IMSSampleApplication.Models
{
    public class ProtocolResponse
    {
        public string version { get; set; }
        public string messageMethod { get; set; }
        public string supportedEncoding { get; set; }
        public string supportedCharacterSet { get; set; }
        public bool hasDiscovery { get; set; }

        public ProtocolResponse(string version, string messageMethod, string supportedEncoding, string supportedCharacterSet, bool hasDiscovery)
        {
            this.version = version;
            this.messageMethod = messageMethod;
            this.supportedEncoding = supportedEncoding;
            this.supportedCharacterSet = supportedCharacterSet;
            this.hasDiscovery = hasDiscovery;
        }
    }
}
