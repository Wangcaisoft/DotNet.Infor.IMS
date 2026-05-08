using System;
using System.IO;
using System.IO.Compression;

namespace IMSSampleApplication.Helpers
{
    public static class CompressionHelper
    {
        private static int MAGIC_1 = 0x78;
        private static int MAGIC_2A = 0x01;
        private static int MAGIC_2B = 0x5e;
        private static int MAGIC_2C = 0x9c;
        private static int MAGIC_2D = 0xda;
        private static int DEFLATED_SIGNATURE_HEADER_LENGTH = 3;

        public static bool isDataInDeflatedFormat(this byte[] payloadContent)
        {
            if (payloadContent != null && payloadContent.Length > DEFLATED_SIGNATURE_HEADER_LENGTH)
            {
                byte[] signature = copyOfRange(payloadContent, 0, DEFLATED_SIGNATURE_HEADER_LENGTH);
                return matchesDeflateSignature(signature, signature.Length);
            }
            return false;
        }

        public static byte[] copyOfRange(byte[] src, int start, int end)
        {
            int len = end - start;
            byte[] dest = new byte[len];
            Array.Copy(src, start, dest, 0, len);
            return dest;
        }

        private static bool matchesDeflateSignature(byte[] signature, int length)
        {
            return length >= DEFLATED_SIGNATURE_HEADER_LENGTH && signature[0] == MAGIC_1 && (
                       signature[1] == (byte)MAGIC_2A
                       || signature[1] == (byte)MAGIC_2B
                       || signature[1] == (byte)MAGIC_2C
                       || signature[1] == (byte)MAGIC_2D);
        }

        public static byte[] Decompress(byte[] data)
        {
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            // skip zlib header bytes if present
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
}
