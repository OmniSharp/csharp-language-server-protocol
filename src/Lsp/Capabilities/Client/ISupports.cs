using System;

namespace Lsp.Capabilities.Client
{
    public interface ISupports
    {
        bool IsSupported { get; }
        Type ValueType { get; }
        object Value { get; }
    }
}