using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class ProposedExtensions
    {
        /// <summary>
        /// Enables proposals within serialization
        /// </summary>
        /// <returns></returns>
        public static LanguageClientOptions EnableProposals(this LanguageClientOptions options)
        {
            options.Serializer = new ProposedLspSerializer();
            return options;
        }

        /// <summary>
        /// Enables proposals within serialization
        /// </summary>
        /// <returns></returns>
        public static LanguageServerOptions EnableProposals(this LanguageServerOptions options)
        {
            options.Serializer = new ProposedLspSerializer();
            return options;
        }
    }
}
