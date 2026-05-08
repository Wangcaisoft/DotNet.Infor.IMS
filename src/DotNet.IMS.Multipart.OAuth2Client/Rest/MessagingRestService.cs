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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace IMSSampleFlow.Rest
{
    class MessagingRestService
    {
        /** ION registered IONAPI IMS application URL. */
        private string IMS_URL;

        private string TENANT_ID;

        public MessagingRestService(string imsUrl, string tenantId)
        {
            this.IMS_URL = imsUrl;
            this.TENANT_ID = tenantId;
        }
        private HttpResponseMessage execute(string url, string token, MultipartContent bodyContent, Boolean isTenantHeaderReq)
        {
            try
            {
                HttpResponseMessage result = null;
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                    if (isTenantHeaderReq)
                    {
                        client.DefaultRequestHeaders.Add("X-TenantId", TENANT_ID);
                    }
                    if (null != bodyContent)
                    {
                        result = client.PostAsync(url, bodyContent).Result;
                    }
                    else
                    {
                        result = client.GetAsync(url).Result;
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error occured while executing the rest call '" + url + "'", e);
            }
        }

        public void sendMultipartMessage(string token)
        {
           
            string parameterFile = MessageHelper.getParameterFile();
            MemoryStream messagePayloadStream = MessageHelper.getMessagePayload();

            string contentID = Guid.NewGuid().ToString();

            // Creating one of the multipart request parameter 'ParameterRequest'.
            System.Net.Http.HttpContent paramContent = new StringContent(parameterFile);
            paramContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            paramContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "ParameterRequest" };

            // Creating one of the multipart request parameter 'MessagePayload'.
            StreamContent bodyContent = new StreamContent(messagePayloadStream);
            bodyContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            bodyContent.Headers.Add("Content-Transfer-Encoding", "binary");
            bodyContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "MessagePayload" };

            // Create multipart request and adding two parameters(ParameterRequest and MessagePayload) which are created in the above steps.
            MultipartContent content = new MultipartContent("form-data", contentID);
            content.Add(paramContent);
            content.Add(bodyContent);

            HttpResponseMessage result = execute(IMS_URL + "/v3/multipartMessage", token, content, true);
            int statusCode = (int)result.StatusCode;
            var responseBody = result.Content.ReadAsStringAsync().Result;
            if (statusCode == 202)
            {
                Console.WriteLine("Multipart message delivered successfully.");
                Console.WriteLine("Received response: " + responseBody);
            }
            else if (statusCode == 401 || statusCode == 403 || statusCode == 404 || statusCode == 406 || statusCode == 408 || statusCode == 415 || statusCode == 500 || statusCode == 502 || statusCode == 503 || statusCode == 504)
            {
                Console.WriteLine("Send multipart message failed with response: " + responseBody);
                Console.WriteLine("Something problem in request information or at server side. Stopping the message sending process(retry logic here in case of original message sending process).");
                Console.ReadKey();
                System.Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Send multipart message failed with response: " + responseBody);
                Console.ReadKey();
                System.Environment.Exit(0);
            }

            messagePayloadStream.Close();
        }

        public void ping(string token)
        {
            HttpResponseMessage result = execute(IMS_URL + "/ping", token, null, false);

            int statusCode = (int)result.StatusCode;
            var responseBody = result.Content.ReadAsStringAsync().Result;
            if (statusCode == 200)
            {
                Console.WriteLine("Ping succeeded with response: " + responseBody);
            }
            else if (statusCode == 401 || statusCode == 403 || statusCode == 404 || statusCode == 406 || statusCode == 408 || statusCode == 415 || statusCode == 500 || statusCode == 502 || statusCode == 503 || statusCode == 504)
            {
                Console.WriteLine("Something problem in request information or at server side. Stopping the message sending process(retry logic here in case of original message sending process).");
                Console.WriteLine("Received response body: " + responseBody);
                Console.ReadKey();
                System.Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Ping failed. Stopping the message sending process.");
                Console.WriteLine("Received response body: " + responseBody);
                Console.ReadKey();
                System.Environment.Exit(0);
            }
        }

        public void getSupportedVersions(string token)
        {
            HttpResponseMessage result = execute(IMS_URL + "/versions", token, null, false);
            try
            {
                List<string> supportedVersions = null;
                int statusCode = (int)result.StatusCode;
                var responseBody = result.Content.ReadAsStringAsync().Result;
                if (statusCode == 200)
                {
                    Console.WriteLine("Versions call succeeded with response: " + responseBody);

                    Newtonsoft.Json.Linq.JObject jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                    var versions = jsonResponse.SelectToken("supportedVersions");
                    if (null != versions)
                    {
                        supportedVersions = versions.ToObject<List<string>>();
                    }
                    if (null == versions || !supportedVersions.Contains("v3"))
                    {
                        Console.WriteLine("Minimum version required to run this project is v3..");
                        Console.WriteLine("\nStopping the message sending process.");
                        Console.ReadKey();
                        System.Environment.Exit(0);
                    }
                }
                else
                {
                    Console.WriteLine("Versions call failed with response: " + responseBody);
                    Console.WriteLine("\nStopping the message sending process.");
                    Console.ReadKey();
                    System.Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while processing the versions response.", e);
            }
        }

        public void acceptedDocuments(string token)
        {
            string documentName = null;
            string fromLogicalId = null;
            try
            {
                string assemblyLocation = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                // Getting the documentName and fromLogicalId from message parameter file for the validation.
                string parameterRequestFile = System.IO.File.ReadAllText(assemblyLocation + "\\Data\\MessageParameters.json");
                Newtonsoft.Json.Linq.JObject parameterRequest = Newtonsoft.Json.Linq.JObject.Parse(parameterRequestFile);
                documentName = (string)parameterRequest.SelectToken("documentName");
                fromLogicalId = (string)parameterRequest.SelectToken("fromLogicalId");
            } catch(Exception e)
            {
                throw new Exception("Error occured while reading the properties from message parameters.", e);
            }

            if (null == fromLogicalId)
            {
                throw new Exception("Please provide the fromLogicalId in ParameterRequest file.");
            }
            if (null == documentName)
            {
                throw new Exception("Please provide the documentName in ParameterRequest file.");
            }

            string encodedLogicalId = System.Web.HttpUtility.UrlEncode(fromLogicalId);

            var acceptedDocUrl = IMS_URL + "/v3/" + encodedLogicalId + "/acceptedDocuments";
            HttpResponseMessage acceptedDocResult = execute(acceptedDocUrl, token, null, true);

            try
            {
                string responseBody = acceptedDocResult.Content.ReadAsStringAsync().Result;

                int statusCode = (int)acceptedDocResult.StatusCode;
                if (statusCode == 200)
                {
                    Newtonsoft.Json.Linq.JObject jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(responseBody);
                    List<string> acceptedDocuments = jsonResponse.SelectToken("acceptedDocuments").ToObject<List<string>>();
                    string logicalId = (string)jsonResponse.SelectToken("logicalId");

                    Console.WriteLine("acceptedDocuments call succeeded with response: " + responseBody);
                    if (!acceptedDocuments.Contains(documentName) || !logicalId.Equals(fromLogicalId))
                    {
                        Console.WriteLine("The document '" + documentName + "' is not configured for the sending application '" + logicalId + "'. Please configure and try again later.");
                        Console.WriteLine("\nStopping the message sending process.");
                        Console.ReadKey();
                        System.Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("The document '" + documentName + "' is configured for the sending application '" + logicalId + "'.");
                    }
                }
                else
                {
                    Console.WriteLine("acceptedDocuments call failed with response: " + responseBody);
                    Console.ReadKey();
                    System.Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while processing the accepted documents response.", e);
            }
        }
    }
}
