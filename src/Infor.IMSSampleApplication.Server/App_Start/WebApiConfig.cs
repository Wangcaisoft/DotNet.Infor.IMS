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

using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace IMSSampleApplication
{
    public static class WebApiConfig
    {
        // Configure Web API for both IIS and self-hosting.
        // Do NOT start any HttpSelfHostServer here — this method is called by
        // the hosting environment (IIS) on Application_Start. Self-hosting
        // should be performed by a dedicated host process which calls
        // SelfHostStarter.Start(...) so listeners are not opened inside IIS.
        public static void Register(HttpConfiguration config)
        {
            // Use the provided configuration (IIS host will supply this).
            config.MapHttpAttributeRoutes();

            config.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter(new MultipartFormatterSettings()));
        }
    }
}
