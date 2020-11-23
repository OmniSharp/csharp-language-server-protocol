using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using TestingUtils;
using Xunit;

namespace Generation.Tests
{
    public class LspFeatureTests
    {
//        [FactWithSkipOn(SkipOnPlatform.Windows, Skip = "for testing")]
        [Fact]
        public async Task Supports_Params_Type_As_Source()
        {
            var source = FeatureFixture.ReadSource("Workspace.WorkspaceSymbolsFeature.cs");
            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Bogus;
using OmniSharp.Extensions.LanguageServer.Protocol.Bogus.Handlers;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using RenameCapability = OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.RenameCapability;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ITextDocumentIdentifierParams = OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentIdentifierParams;
using RenameRegistrationOptions = OmniSharp.Extensions.LanguageServer.Protocol.Models.RenameRegistrationOptions;
using WorkspaceEdit = OmniSharp.Extensions.LanguageServer.Protocol.Models.WorkspaceEdit;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Bogus.Handlers
{
    [Parallel]
    [Method(TextDocumentNames.Rename, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IRenameHandler : IJsonRpcRequestHandler<RenameParams, WorkspaceEdit?>, IRegistration<RenameRegistrationOptions, RenameCapability>
    {
    }

    [Parallel]
    [Method(TextDocumentNames.Rename, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class RenameHandlerBase : AbstractHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Bogus.Handlers
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class RenameExtensions
    {
        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, Task<WorkspaceEdit?>> handler, Func<RenameCapability, RenameRegistrationOptions> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, Task<WorkspaceEdit?>> handler, Func<RenameRegistrationOptions> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, Task<WorkspaceEdit?>> handler, RenameRegistrationOptions registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, CancellationToken, Task<WorkspaceEdit?>> handler, Func<RenameCapability, RenameRegistrationOptions> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, CancellationToken, Task<WorkspaceEdit?>> handler, Func<RenameRegistrationOptions> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, CancellationToken, Task<WorkspaceEdit?>> handler, RenameRegistrationOptions registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, RenameCapability, CancellationToken, Task<WorkspaceEdit?>> handler, Func<RenameCapability, RenameRegistrationOptions> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, RenameCapability, CancellationToken, Task<WorkspaceEdit?>> handler, Func<RenameRegistrationOptions> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, RenameCapability, CancellationToken, Task<WorkspaceEdit?>> handler, RenameRegistrationOptions registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(handler, RegistrationOptionsFactoryAdapter.Adapt<RenameRegistrationOptions, RenameCapability>(registrationOptions)));
        }

        public static Task<WorkspaceEdit?> RequestRename(this ITextDocumentLanguageClient mediator, RenameParams request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        public static Task<WorkspaceEdit?> RequestRename(this ILanguageClient mediator, RenameParams request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}";
            await GenerationHelpers.AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }
    }
}
