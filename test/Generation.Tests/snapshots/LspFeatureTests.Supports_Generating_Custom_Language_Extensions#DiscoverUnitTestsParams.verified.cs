//HintName: DiscoverUnitTestsParams.cs
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
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Lsp.Tests.Integration.Fixtures
{
    [Parallel, Method("tests/discover", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IDiscoverUnitTestsHandler : IJsonRpcRequestHandler<DiscoverUnitTestsParams, Container<UnitTest>>, IRegistration<UnitTestRegistrationOptions, UnitTestCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class DiscoverUnitTestsHandlerBase : AbstractHandlers.Request<DiscoverUnitTestsParams, Container<UnitTest>, UnitTestRegistrationOptions, UnitTestCapability>, IDiscoverUnitTestsHandler
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class DiscoverUnitTestsPartialHandlerBase : AbstractHandlers.PartialResults<DiscoverUnitTestsParams, Container<UnitTest>, UnitTest, UnitTestRegistrationOptions, UnitTestCapability>, IDiscoverUnitTestsHandler
    {
        protected DiscoverUnitTestsPartialHandlerBase(System.Guid id, IProgressManager progressManager) : base(progressManager, Container<UnitTest>.From)
        {
        }
    }
}
#nullable restore

namespace Lsp.Tests.Integration.Fixtures
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class DiscoverUnitTestsExtensions
    {
        public static ILanguageServerRegistry OnDiscoverUnitTests(this ILanguageServerRegistry registry, Func<DiscoverUnitTestsParams, Task<Container<UnitTest>>> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/discover", new LanguageProtocolDelegatingHandlers.Request<DiscoverUnitTestsParams, Container<UnitTest>, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, Container<UnitTest>>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDiscoverUnitTests(this ILanguageServerRegistry registry, Func<DiscoverUnitTestsParams, CancellationToken, Task<Container<UnitTest>>> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/discover", new LanguageProtocolDelegatingHandlers.Request<DiscoverUnitTestsParams, Container<UnitTest>, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, Container<UnitTest>>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDiscoverUnitTests(this ILanguageServerRegistry registry, Func<DiscoverUnitTestsParams, UnitTestCapability, CancellationToken, Task<Container<UnitTest>>> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/discover", new LanguageProtocolDelegatingHandlers.Request<DiscoverUnitTestsParams, Container<UnitTest>, UnitTestRegistrationOptions, UnitTestCapability>(HandlerAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, Container<UnitTest>>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveDiscoverUnitTests(this ILanguageServerRegistry registry, Action<DiscoverUnitTestsParams, IObserver<IEnumerable<UnitTest>>> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/discover", _ => new LanguageProtocolDelegatingHandlers.PartialResults<DiscoverUnitTestsParams, Container<UnitTest>, UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(PartialAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<UnitTest>.From));
        }

        public static ILanguageServerRegistry ObserveDiscoverUnitTests(this ILanguageServerRegistry registry, Action<DiscoverUnitTestsParams, IObserver<IEnumerable<UnitTest>>, CancellationToken> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/discover", _ => new LanguageProtocolDelegatingHandlers.PartialResults<DiscoverUnitTestsParams, Container<UnitTest>, UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(PartialAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<UnitTest>.From));
        }

        public static ILanguageServerRegistry ObserveDiscoverUnitTests(this ILanguageServerRegistry registry, Action<DiscoverUnitTestsParams, IObserver<IEnumerable<UnitTest>>, UnitTestCapability, CancellationToken> handler, RegistrationOptionsDelegate<UnitTestRegistrationOptions, UnitTestCapability> registrationOptions)
        {
            return registry.AddHandler("tests/discover", _ => new LanguageProtocolDelegatingHandlers.PartialResults<DiscoverUnitTestsParams, Container<UnitTest>, UnitTest, UnitTestRegistrationOptions, UnitTestCapability>(PartialAdapter<UnitTestCapability>.Adapt<DiscoverUnitTestsParams, UnitTest>(handler), RegistrationAdapter<UnitTestCapability>.Adapt<UnitTestRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<UnitTest>.From));
        }

        public static IRequestProgressObservable<IEnumerable<UnitTest>, Container<UnitTest>> RequestDiscoverUnitTests(this ILanguageClient mediator, DiscoverUnitTestsParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, Container<UnitTest>.From, cancellationToken);
    }
#nullable restore
}