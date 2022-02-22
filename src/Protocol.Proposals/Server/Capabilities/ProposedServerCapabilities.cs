using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Is not a record on purpose...
    /// get; set; for the moment, to allow for replacement of values.
    /// </remarks>
    [Obsolete(Constants.Proposal)]
    public class ProposedServerCapabilities : ServerCapabilities
    {
    }
}
