using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Call hierarchy options used during static or dynamic registration.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyRegistrationOptions : StaticWorkDoneTextDocumentRegistrationOptions, ICallHierarchyOptions
    {
    }
}
