using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Call hierarchy options used during static registration.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyOptions : WorkDoneProgressOptions, ICallHierarchyOptions
    {
        public static CallHierarchyOptions Of(ICallHierarchyOptions options)
        {
            return new CallHierarchyOptions() {
                WorkDoneProgress = options.WorkDoneProgress,
            };
        }
    }
}
