using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    public interface ISemanticTokenResult
    {
        /// <summary>
        /// An optional result id. If provided and clients support delta updating
        /// the client will include the result id in the next semantic token request.
        /// A server can then instead of computing all semantic tokens again simply
        /// send a delta.
        /// </summary>
        [Optional]
        public string ResultId { get; set; }
    }
}
