using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ISupports
    {
        bool IsSupported { get; }
        Type ValueType { get; }
        object Value { get; }
    }

    /// <summary>
    /// This is a marker interface so that the foundation tests can correctly ignore the return types of some methods.
    /// </summary>
    public interface ISupportingHandler {}
}
