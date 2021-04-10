// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
#if NETSTANDARD
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ModuleInitializerAttribute : Attribute { }
#endif
}
