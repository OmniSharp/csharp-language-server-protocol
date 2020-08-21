using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Call hierarchy options used during static or dynamic registration.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyRegistrationOptions : StaticWorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// Call hierarchy options used during static registration.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public class StaticOptions : WorkDoneProgressOptions
        {
        }

        class CallHierarchyRegistrationOptionsConverter : RegistrationOptionsConverterBase<CallHierarchyRegistrationOptions, StaticOptions>
        {
            public CallHierarchyRegistrationOptionsConverter() : base(nameof(ServerCapabilities.CallHierarchyProvider))
            {
            }
            public override StaticOptions Convert(CallHierarchyRegistrationOptions source)
            {
                return new StaticOptions {
                    WorkDoneProgress = source.WorkDoneProgress
                };
            }
        }
    }
}
