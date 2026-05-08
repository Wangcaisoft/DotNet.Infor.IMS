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


using IMSSampleFlow.Helpers;
using IMSSampleFlow.Rest;
using System;


namespace IMSSampleApplication
{
    class IMSOAuth2ClientApp
    {
        private static string IMS_URL = "{0}/{1}/IONSERVICES/api/ion/messaging/service";

        static void Main(string[] args)
        {
            try
            {
                OAuthHelper oauthHelper = new OAuthHelper();
                string token = oauthHelper.getToken();
                IMS_URL = string.Format(IMS_URL, oauthHelper.getBaseUrl(), oauthHelper.getTenantId());
                MessagingRestService restService = new MessagingRestService(IMS_URL, oauthHelper.getTenantId());

                Console.WriteLine("Starting the message sending process:");

                Console.WriteLine("1. Calling the ping service...");
                restService.ping(token);

                Console.WriteLine("\n2. Calling the version service...");
                restService.getSupportedVersions(token);

                Console.WriteLine("\n3. Calling the acceptedDocuments service...");
                restService.acceptedDocuments(token);

                Console.WriteLine("\n4. Calling the multipartMessage service...");
                restService.sendMultipartMessage(token);

                Console.ReadKey();
                System.Environment.Exit(0);
            }  catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.ReadKey();
                System.Environment.Exit(0);
            }
        }
    }
}
