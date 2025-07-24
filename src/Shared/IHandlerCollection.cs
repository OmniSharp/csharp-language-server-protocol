using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal interface IHandlerCollection : IHandlersManager, IEnumerable<ILspHandlerDescriptor>
    {
        bool ContainsHandler(Type type);
        bool ContainsHandler(TypeInfo typeInfo);
    }
}
