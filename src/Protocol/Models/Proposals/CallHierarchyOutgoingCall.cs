using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Represents an outgoing call, e.g. calling a getter from a method or a method from a constructor etc.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class CallHierarchyOutgoingCall
    {
        /// <summary>
        /// The item that is called.
        /// </summary>
        public CallHierarchyItem To { get; set; } = null!;

        /// <summary>
        /// The range at which this item is called. This is the range relative to the caller, e.g the item
        /// passed to [`provideCallHierarchyOutgoingCalls`](#CallHierarchyItemProvider.provideCallHierarchyOutgoingCalls)
        /// and not [`this.to`](#CallHierarchyOutgoingCall.to).
        /// </summary>
        public Container<Range> FromRanges { get; set; } = null!;
    }
}
