using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class LspRequestContext : ILspRequestContext
    {
        public ILspHandlerDescriptor Descriptor { get; set; }
    }
}