//HintName: AssemblyCapabilityKeys.cs
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: AssemblyCapabilityKey(typeof(OmniSharp.Extensions.LanguageServer.Protocol.Test.Client.Capabilities.OutlayHintWorkspaceClientCapabilities), nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.OutlayHint))]