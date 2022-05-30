//HintName: AssemblyCapabilityKeys.cs
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Diagnostics;

[assembly: AssemblyCapabilityKey(typeof(OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.WorkspaceSymbolCapability), nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.Symbol))]