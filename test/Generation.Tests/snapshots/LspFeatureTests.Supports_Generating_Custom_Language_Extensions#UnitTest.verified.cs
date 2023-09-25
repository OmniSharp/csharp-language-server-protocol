//HintName: UnitTest.cs
using Lsp.Tests.Integration.Fixtures;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Lsp.Tests.Integration.Fixtures
{
    [Parallel, Method("tests/run", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IUnitTestHandler : IJsonRpcNotificationHandler<UnitTest>, IRegistration<UnitTestRegistrationOptions, UnitTestCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class UnitTestHandlerBase : AbstractHandlers.Notification<UnitTest, UnitTestRegistrationOptions, UnitTestCapability>, IUnitTestHandler
    {
    }
}
#nullable restore

namespace Lsp.Tests.Integration.Fixtures
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class UnitTestExtensions
    {
        public static ILanguageServerRegistry OnUnitTest(this ILanguageServerRegistry registry, Action<UnitTest> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/run", new LanguageProtocolDelegatingHandlers.Notification<UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnUnitTest(this ILanguageServerRegistry registry, Func<UnitTest, Task> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/run", new LanguageProtocolDelegatingHandlers.Notification<UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnUnitTest(this ILanguageServerRegistry registry, Action<UnitTest, CancellationToken> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/run", new LanguageProtocolDelegatingHandlers.Notification<UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnUnitTest(this ILanguageServerRegistry registry, Func<UnitTest, CancellationToken, Task> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/run", new LanguageProtocolDelegatingHandlers.Notification<UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnUnitTest(this ILanguageServerRegistry registry, Action<UnitTest, UnitTestCapability, CancellationToken> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/run", new LanguageProtocolDelegatingHandlers.Notification<UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnUnitTest(this ILanguageServerRegistry registry, Func<UnitTest, UnitTestCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/run", new LanguageProtocolDelegatingHandlers.Notification<UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static void SendUnitTest(this ILanguageClient mediator, UnitTest request) => mediator.SendNotification(request);
    }
#nullable restore
}