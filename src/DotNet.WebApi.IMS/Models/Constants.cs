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

namespace IMSSampleApplication.Models
{
    public class Constants
    {
        public static string MSG_RESPONSE_202 = "The request was processed successfully";

        public static string MSG_RESPONSE_450 = "Given document value is not in the specified encoding format [DEFLATE]. Reason : Received unexpected deflate data with 'unexpected deflate header checksum data'.";

        public static string MSG_RESPONSE_453 = "Empty request body not allowed";

        public static string MSG_RESPONSE_418 = "tenantId is required. V1: tenant in the path. V2 and V3: set header X-TenantId";

        public static string MSG_RESPONSE_448 = "tenantId should not be more than 22 characters";

        public static String MSG_RESPONSE_456 = "Invalid tenantId {0}";

        public static string MSG_RESPONSE_477 = "Invalid multipart parameters. Request must contain two parameters, one parameter with name 'ParameterRequest' and another parameter with name 'MessagePayload'.";

        public static string MSG_RESPONSE_470_PARAMETER_INVALIDTYPE = "Multipart message request is invalid. Received ParameterRequest body is not as per specification.";

        public static string MSG_RESPONSE_470_PAYLOAD_INVALIDTYPE = "Multipart message request is invalid. Received MessagePayload body is not as per specification.";
    }
}