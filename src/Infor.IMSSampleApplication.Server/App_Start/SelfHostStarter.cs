/* ---Begin Copyright Notice--- Feb 4, 2021 11:52:46 AM
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
using IMSSampleApplication.App_Start;

namespace IMSSampleApplication.App_Start
{
    // Utility to start a self-hosted HttpSelfHostServer from a dedicated
    // host process (console app or Windows service). Returns the server so
    // the caller controls the lifetime and does not block IIS threads.
    public static class SelfHostStarter
    {
        // Starts and opens a self-host server and returns the server instance.
        // Caller is responsible for disposing the returned HttpSelfHostServer.
        public static HttpSelfHostServer Start(string baseAddress, MultipartFormatterSettings formatterSettings = null)
        {
            if (string.IsNullOrWhiteSpace(baseAddress)) throw new ArgumentNullException(nameof(baseAddress));

            // Use custom MyHttpsSelfHostConfiguration if you need transport security.
            HttpSelfHostConfiguration selfHostConfig = new MyHttpsSelfHostConfiguration(baseAddress);

            selfHostConfig.MapHttpAttributeRoutes();

            selfHostConfig.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter(formatterSettings ?? new MultipartFormatterSettings()));

            var server = new HttpSelfHostServer(selfHostConfig);
            server.OpenAsync().Wait();

            return server;
        }
    }
}
