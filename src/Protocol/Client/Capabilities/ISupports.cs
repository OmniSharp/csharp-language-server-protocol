using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public interface ISupports
    {
        bool IsSupported { get; }
        Type ValueType { get; }
        object Value { get; }
    }
}