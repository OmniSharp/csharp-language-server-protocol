using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

/// <summary>
/// Client capability that signals how the client
/// handles stale requests (e.g. a request
/// for which the client will not process the response
/// anymore since the information is outdated).
///
/// @since 3.17.0
/// </summary>
public class StaleRequestSupportClientCapabilities
{
    /// <summary>
    /// The client will actively cancel the request.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// The list of requests for which the client
    /// will retry the request if it receives a
    /// response with error code `ContentModified``
    /// </summary>
    public Container<string> RetryOnContentModified { get; set; } = new();
}
