﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SourcePresentationHint
    {
        Normal,
        Emphasize,
        Deemphasize,
    }
}