namespace DotNet.IMS.Server;

public static class Constants
{
    public static String MSG_RESPONSE_202 = "The request was processed successfully";
    public static String MSG_RESPONSE_450 = "Given document value is not in the specified encoding format [DEFLATE]. Reason : Received unexpected deflate data with 'unexpected deflate header checksum data'.";
    public static String MSG_RESPONSE_453 = "Empty request body not allowed";
    public static String MSG_RESPONSE_418 = "tenantId is required. V1: tenant in the path. V2 and V3: set header X-TenantId";
    public static String MSG_RESPONSE_448 = "tenantId should not be more than 22 characters";
    public static String MSG_RESPONSE_456 = "Invalid tenantId {0}";
    public static String MSG_RESPONSE_477 = "Invalid multipart parameters. Request must contain two parameters, one parameter with name 'ParameterRequest' and another parameter with name 'MessagePayload'.";
    public static String MSG_RESPONSE_470_PARAMETER_INVALIDTYPE = "Multipart message request is invalid. Received ParameterRequest body is not as per specification.";
    public static String MSG_RESPONSE_470_PAYLOAD_INVALIDTYPE = "Multipart message request is invalid. Received MessagePayload body is not as per specification.";
}
