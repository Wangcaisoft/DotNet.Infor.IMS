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
using System.Web.Http.SelfHost;
using System.Web.Http.SelfHost.Channels;

namespace IMSSampleApplication.App_Start
{
    public class MyHttpsSelfHostConfiguration : HttpSelfHostConfiguration
    {
        public MyHttpsSelfHostConfiguration(string baseAddress) : base(baseAddress) { }
        public MyHttpsSelfHostConfiguration(Uri baseAddress) : base(baseAddress) { }
        protected override System.ServiceModel.Channels.BindingParameterCollection OnConfigureBinding(HttpBinding httpBinding)
        {
            httpBinding.Security.Mode = HttpBindingSecurityMode.Transport;
            return base.OnConfigureBinding(httpBinding);
        }
    }
}