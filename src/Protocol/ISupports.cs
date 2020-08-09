using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ISupports
    {
        bool IsSupported { get; }
        Type ValueType { get; }
        object Value { get; }
    }
}
