/**
 * ---Begin Copyright Notice--- Feb 4 2021
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

namespace IMSSampleFlow.Helpers
{
    class MessageHelper
    {
        static string assemblyLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        public static MemoryStream getMessagePayload()
        {
            System.IO.Compression.DeflateStream deflate = null;
            string messagePayloadFileLoc = assemblyLocation + "\\Data\\MessagePayload";

            MemoryStream ms = new MemoryStream();
            byte[] data = System.IO.File.ReadAllBytes(messagePayloadFileLoc);

            byte[] compatibleHeader = { 0x78, 0x01 };
            ms.Write(compatibleHeader, 0, compatibleHeader.Length);
            
            // Setting deflated message body.
            using (deflate = new DeflateStream(ms, CompressionLevel.Fastest, true))
            {
                deflate.Write(data, 0, data.Length);
                deflate.Flush();
            }

            ms.Write(Adler32(data, 0, data.Length), 0, 4);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        ///   Calculate the Adler32 checksum for zlib use.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] Adler32(byte[] data, int offset, int length)
        {
            uint Modulus = 65521;
            uint a = 1;
            uint b = 0;
            byte[] adlerBytes;
            for (int counter = 0; counter < length; ++counter)
            {
                a = (a + (data[offset + counter])) % Modulus;
                b = (b + a) % Modulus;
            }
            adlerBytes = BitConverter.GetBytes(unchecked((uint)((b << 16) + a)));
            Array.Reverse(adlerBytes);
            return adlerBytes;
        }

        public static string getParameterFile()
        {
            string paramterFileContent = System.IO.File.ReadAllText(assemblyLocation + "\\Data\\MessageParameters.json");
            return paramterFileContent;
        }
    }
}
