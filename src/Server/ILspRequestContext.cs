using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILspRequestContext
    {
        ILspHandlerDescriptor Descriptor { get; set; }
    }
}