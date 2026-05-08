using System.IO.Compression;

namespace DotNet.IMS.Server.Helpers;

public static class CompressionHelper
{
    private static Int32 MAGIC_1 = 0x78;
    private static Int32 MAGIC_2A = 0x01;
    private static Int32 MAGIC_2B = 0x5e;
    private static Int32 MAGIC_2C = 0x9c;
    private static Int32 MAGIC_2D = 0xda;
    private static Int32 DEFLATED_SIGNATURE_HEADER_LENGTH = 3;

    public static Boolean isDataInDeflatedFormat(this Byte[] payloadContent)
    {
        if (payloadContent != null && payloadContent.Length > DEFLATED_SIGNATURE_HEADER_LENGTH)
        {
            var signature = copyOfRange(payloadContent, 0, DEFLATED_SIGNATURE_HEADER_LENGTH);
            return matchesDeflateSignature(signature, signature.Length);
        }
        return false;
    }

    public static Byte[] copyOfRange(Byte[] src, Int32 start, Int32 end)
    {
        var len = end - start;
        var dest = new Byte[len];
        Array.Copy(src, start, dest, 0, len);
        return dest;
    }

    private static Boolean matchesDeflateSignature(Byte[] signature, Int32 length)
    {
        return length >= DEFLATED_SIGNATURE_HEADER_LENGTH && signature[0] == MAGIC_1 && (
                   signature[1] == (Byte)MAGIC_2A
                   || signature[1] == (Byte)MAGIC_2B
                   || signature[1] == (Byte)MAGIC_2C
                   || signature[1] == (Byte)MAGIC_2D);
    }

    public static Byte[] Decompress(Byte[] data)
    {
        using var input = new MemoryStream(data);
        using var output = new MemoryStream();
        if (input.Length > 2)
        {
            input.ReadByte();
            input.ReadByte();
        }
        using (var dstream = new DeflateStream(input, CompressionMode.Decompress))
        {
            dstream.CopyTo(output);
        }
        return output.ToArray();
    }
}
