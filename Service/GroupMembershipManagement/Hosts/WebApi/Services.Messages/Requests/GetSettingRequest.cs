// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Models;
using Services.Messages.Contracts.Requests;

namespace Services.Messages.Requests
{
    public class GetSettingRequest : RequestBase
    {
        public GetSettingRequest(SettingKey settingKey)
        {
            this.SettingKey = settingKey;
        }

        public SettingKey SettingKey { get; }
    }
}