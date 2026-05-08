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
using Thinktecture.IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace IMSSampleFlow.Helpers
{
    class OAuthHelper
    {
        private static string resourceOwnerClientId;

        private static string resourceOwnerClientSecret;

        private static string oAuth2TokenEndpoint;

        private static string serviceAccountAccessKey;

        private static string serviceAccountSecretKey;

        private static string baseUrl;

        private static string tenantId;

        public string getToken()
        {
            readIONAPIProperties();
            TokenResponse token = null;
            try
            {
                OAuth2Client _oauth2 = new OAuth2Client(
                new Uri(oAuth2TokenEndpoint), resourceOwnerClientId, resourceOwnerClientSecret);

                token = RequestToken(_oauth2);

                return token.TokenType + " " + token.AccessToken;
            }
            catch (Exception e)
            {
                Console.WriteLine("Something problem in generating the oauth2 token. Reason: " + e.Message);
                Console.ReadKey();
                System.Environment.Exit(0);
            }
            return null;
        }

        private void readIONAPIProperties()
        {
            string assemblyLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string filePath = assemblyLocation + "\\Data\\test.ionapi";
            if (File.Exists(@filePath))
            {
                string fileContent = System.IO.File.ReadAllText(filePath);
                var details = JObject.Parse(fileContent);
                resourceOwnerClientId = details["ci"].ToString();
                resourceOwnerClientSecret = details["cs"].ToString();
                oAuth2TokenEndpoint = string.Concat(details["pu"].ToString(), details["ot"].ToString());
                serviceAccountAccessKey = details["saak"].ToString();
                serviceAccountSecretKey = details["sask"].ToString();
                tenantId = details["ti"].ToString();
                baseUrl = details["iu"].ToString();
            } else
            {
                Console.WriteLine("'TEST.ionapi' file is not exists. Please make sure the file available in the data folder.");
                Console.ReadKey();
                System.Environment.Exit(0);
            }
        }

        private static TokenResponse RequestToken(OAuth2Client _oauth2)
        {
            return _oauth2.RequestResourceOwnerPasswordAsync
                (serviceAccountAccessKey, serviceAccountSecretKey).Result;
        }

        public String getBaseUrl()
        {
            return baseUrl;
        }

        public String getTenantId()
        {
            return tenantId;
        }
    }
}
