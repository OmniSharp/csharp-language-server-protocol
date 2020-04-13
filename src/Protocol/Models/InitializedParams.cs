using System;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Initialized)]
    public class InitializedParams : IRequest
    {
    }
}
