#pragma warning disable MA0048 // File name must match type name
#define INTERNAL_RECORD_ATTRIBUTES
#if NETSTANDARD2_0 ||  NETSTANDARD2_1 ||  NETCOREAPP2_0 ||  NETCOREAPP2_1 ||  NETCOREAPP2_2 || NET45 || NET451 || NET452 || NET6 || NET461 || NET462 || NET47 || NET471 || NET472 || NET48
using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
#if INTERNAL_RECORD_ATTRIBUTES
    internal
#else
    public
#endif
    static class IsExternalInit
    {
    }
}
#endif
