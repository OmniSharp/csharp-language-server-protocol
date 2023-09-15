//HintName: AssemblyCapabilityKeys.cs
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using System.Diagnostics;
using System.Linq;

[assembly: AssemblyCapabilityKey(typeof(OmniSharp.Extensions.LanguageServer.Protocol.Test.Client.Capabilities.SubLensCapability), nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.CodeLens)), AssemblyCapabilityKey(typeof(OmniSharp.Extensions.LanguageServer.Protocol.Test.Client.Capabilities.SubLensWorkspaceClientCapabilities), nameof(ClientCapabilities.Workspace), nameof(WorkspaceClientCapabilities.CodeLens))]