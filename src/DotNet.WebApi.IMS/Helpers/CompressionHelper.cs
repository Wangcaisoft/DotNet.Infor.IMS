/**
 * ---Begin Copyright Notice--- Feb 4, 2021 11:52:46 AM
 *
 * Copyright 2021 Infor
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * ---End Copyright Notice---
 */


using System;
using System.IO;
using System.IO.Compression;

namespace IMSSampleApplication.Helpers
{
    public static class CompressionHelper
    {
        /** Deflate MAGIC HEADERS.. */
        private static int MAGIC_1 = 0x78;

        /** Deflate MAGIC HEADERS.. */
        private static int MAGIC_2A = 0x01;

        /** Deflate MAGIC HEADERS.. */
        private static int MAGIC_2B = 0x5e;

        /** Deflate MAGIC HEADERS.. */
        private static int MAGIC_2C = 0x9c;

        /** Deflate MAGIC HEADERS.. */
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
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            input.ReadByte();
            input.ReadByte();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}