using System;

namespace OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client
{
    public interface ISupports
    {
        bool IsSupported { get; }
        Type ValueType { get; }
        object Value { get; }
    }
}