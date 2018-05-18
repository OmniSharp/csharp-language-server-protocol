using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    using Newtonsoft.Json.Linq;
    using static WorkspaceNames;
    public static partial class WorkspaceNames
    {
        public const string WorkspaceConfiguration = "workspace/configuration";
    }

    [Parallel, Method(WorkspaceConfiguration)]
    public interface IConfigurationHandler : IRequestHandler<ConfigurationParams, Container<JToken>>, IRegistration<WorkspaceFolderRegistrationOptions>, ICapability<WorkspaceFolderCapability> { }
}
