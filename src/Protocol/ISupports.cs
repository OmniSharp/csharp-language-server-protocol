using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ISupports
    {
        bool IsSupported { get; }
        Type ValueType { get; }
        object Value { get; }
    }
}
