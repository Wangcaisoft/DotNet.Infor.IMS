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



using IMSSampleApplication.Helpers;
using IMSSampleApplication.Models;
using MultipartDataMediaFormatter.Infrastructure;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using static MultipartDataMediaFormatter.Infrastructure.FormData;
using System.Text.RegularExpressions;

namespace IMSSampleApplication.Controllers
{
    public class ServiceController : ApiController
    {
        private string validTenantRegex = "^[A-Z0-9]{1,17}_[A-Z0-9]{3}$";

        [HttpGet]
        [Route("ping")]
        public HttpResponseMessage ping()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            PingResponse pingResponse = new PingResponse(200, "OK");
            string jsonResponse = JsonConvert.SerializeObject(pingResponse);

            response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json"),
            };
            return response;
        }

        [HttpGet]
        [Route("protocol")]
        public HttpResponseMessage protocol()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            ProtocolResponse protocolResponse = new ProtocolResponse("v3", "multipartMessage", "DEFLATE", "UTF-8", false);
            string jsonResponse = JsonConvert.SerializeObject(protocolResponse);

            response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json"),
            };
            return response;
        }

        [HttpPost]
        [Route("v3/multipartMessage")]
        public HttpResponseMessage multipartMessage(FormData multipartBody)
        {
            if (multipartBody == null)
            {
                return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 477, Constants.MSG_RESPONSE_477);
            }
            List<string> keys = multipartBody.GetAllKeys();
            if (keys.Count != 2)
            {
                return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 477, Constants.MSG_RESPONSE_477);
            }

            HttpResponseMessage validateTenantresponse = validateTenantId();
            if (validateTenantresponse != null) return validateTenantresponse;

            bool parameterRequestFound = false;
            bool messagePayloadFound = false;
            if (multipartBody.Fields != null)
            {
                List<ValueString> fields = multipartBody.Fields;
                foreach (ValueString field in fields)
                {
                    if ("ParameterRequest".Equals(field.Name))
                    {
                        string parameterRequest = field.Value;
                        if (parameterRequest.Length == 0)
                        {
                            return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 453, Constants.MSG_RESPONSE_453);
                        }
                        Console.WriteLine("Received parameter request: " + parameterRequest);
                        parameterRequestFound = true;
                    }
                    if ("MessagePayload".Equals(field.Name))
                    {
                        byte[] payloadContent = Encoding.UTF8.GetBytes(field.Value);
                        HttpResponseMessage response = processMessagePayload(payloadContent);
                        if (response != null) return response;
                        messagePayloadFound = true;
                    }
                }
                
            }

            for (int i = 0; i < multipartBody.Files.Count(); i++)
            {
                if (!"ParameterRequest".Equals(multipartBody.Files[i].Name) && !"MessagePayload".Equals(multipartBody.Files[i].Name))
                {
                    return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 477, Constants.MSG_RESPONSE_477);
                }
                if ("ParameterRequest".Equals(multipartBody.Files[i].Name) && !"application/json".Equals(multipartBody.Files[i].Value.MediaType))
                {
                    return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 470, Constants.MSG_RESPONSE_470_PARAMETER_INVALIDTYPE);
                } else if ("MessagePayload".Equals(multipartBody.Files[i].Name) && !"application/octet-stream".Equals(multipartBody.Files[i].Value.MediaType))
                {
                    return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 470, Constants.MSG_RESPONSE_470_PAYLOAD_INVALIDTYPE);
                }

                if ("ParameterRequest".Equals(multipartBody.Files[i].Name))
                {
                    byte[] parameterContent = multipartBody.Files[i].Value.Buffer;
                    string parameterRequest = Encoding.UTF8.GetString(parameterContent, 0, parameterContent.Length);
                    if (parameterContent.Length == 0)
                    {
                        return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 453, Constants.MSG_RESPONSE_453);
                    }
                    Console.WriteLine("Received parameter request: " + parameterRequest);
                    parameterRequestFound = true;
                }
                if ("MessagePayload".Equals(multipartBody.Files[i].Name))
                {
                    byte[] payloadContent = multipartBody.Files[i].Value.Buffer;
                    HttpResponseMessage response = processMessagePayload(payloadContent);
                    messagePayloadFound = true;
                    if (response != null) return response;
                }
            }
            if (!parameterRequestFound || !messagePayloadFound)
            {
                return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 477, Constants.MSG_RESPONSE_477);
            }
            return buildMessageResponse("OK", HttpStatusCode.OK, 202, Constants.MSG_RESPONSE_202);
        }

        private HttpResponseMessage processMessagePayload(byte[] payloadContent)
        {
            Uri assemblyLocation = new Uri(Path.GetDirectoryName(Assembly.GetAssembly(typeof(ServiceController)).CodeBase));
            string writeLocation = Directory.GetParent(assemblyLocation.LocalPath).ToString() + "\\Data\\MessagePayload";

            if (!CompressionHelper.isDataInDeflatedFormat(payloadContent))
            {
                return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 450, Constants.MSG_RESPONSE_450);
            }
            else
            {
                byte[] payloadContentInflated = CompressionHelper.Decompress(payloadContent);
                File.WriteAllBytes(writeLocation, payloadContentInflated);
            }
            return null;
        }

        private HttpResponseMessage buildMessageResponse(string statusEnum, HttpStatusCode httpStatus, int code, string msg)
        {
            MultipartMessageResponse messageResponse = new MultipartMessageResponse(statusEnum, code, msg);
            HttpResponseMessage response = Request.CreateResponse(httpStatus);
            string jsonResponse = JsonConvert.SerializeObject(messageResponse);

            response.Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json");

            return response;
        }

        public HttpResponseMessage validateTenantId()
        {
            string tenantId = null;
            HttpRequestHeaders headers = this.Request.Headers;

            if (headers.Contains("X-TenantId")) tenantId = headers.GetValues("X-TenantId").First();

            if (String.IsNullOrEmpty(tenantId))
            {
                return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 418, Constants.MSG_RESPONSE_418);
            }
            if (tenantId.Length > 22)
            {
                return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 448, Constants.MSG_RESPONSE_448);
            }
            Match m = Regex.Match(tenantId, validTenantRegex, RegexOptions.IgnoreCase);
            if (!m.Success && !tenantId.Equals("INFOR", StringComparison.InvariantCultureIgnoreCase))
            {
                // In case of cloud and enterprise connectors, it won't throw this exception.
                return buildMessageResponse("FAIL", HttpStatusCode.PreconditionFailed, 458, String.Format(Constants.MSG_RESPONSE_456, tenantId));
            }
            return null;
        }
    }
}
