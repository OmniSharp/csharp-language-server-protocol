using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Converters
{
    class LowercaseStringEnumConverter : StringEnumConverter
    {
        public LowercaseStringEnumConverter() : base(true) { }
    }
}
