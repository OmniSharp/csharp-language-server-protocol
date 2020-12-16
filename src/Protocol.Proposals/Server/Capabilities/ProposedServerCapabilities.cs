using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// Is not a record on purpose...
    /// get; set; for the moment, to allow for replacement of values.
    /// </remarks>
    public class ProposedServerCapabilities : ServerCapabilities
    {
    }
}
