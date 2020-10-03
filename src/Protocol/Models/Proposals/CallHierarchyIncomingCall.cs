using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Represents an incoming call, e.g. a caller of a method or constructor.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyIncomingCall
    {
        /// <summary>
        /// The item that makes the call.
        /// </summary>
        public CallHierarchyItem From { get; set; } = null!;

        /// <summary>
        /// The range at which at which the calls appears. This is relative to the caller
        /// denoted by [`this.from`](#CallHierarchyIncomingCall.from).
        /// </summary>
        public Container<Range> FromRanges { get; set; } = null!;
    }
}
