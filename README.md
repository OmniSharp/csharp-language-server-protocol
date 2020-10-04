# C# Language Server Protocol

This is an implementation of the [Language Server Protocol](https://github.com/Microsoft/language-server-protocol) written entirely in C# for .NET.

# Getting Started
1. git clone
2. run `build.ps1` / `build.sh`
3. ...
4. Profit

# Concepts

## JSON-RPC
We have an implementation of JSON-RPC designed to implement the [JSON-RPC](https://www.jsonrpc.org/specification) as correctly as possible.

For more information about using the `JsonRpcServer` on it's own [here](./docs/jsonrpc.md).

## MediatR
Internally this library revolves around the request and response model.  To make things easier we use [MediatR](https://github.com/jbogard/MediatR) as core piece that manages how requests and responses are handled.

## Microsoft.Extensions.*
We re-use some of the common packages used by .NET Core.
* `Microsoft.Extensions.Configuration` for common configuration
* `Microsoft.Extensions.DependencyInjection` for common Dependency Injection abstractions
* `Microsoft.Extensions.Logging` for logging.

## Language Server Protocol
We strive to ensure that we implement all the types, request and notifications that are defined by the [Language Server Protocol](https://microsoft.github.io/language-server-protocol/).  Sometimes this is difficult due to the nature of LSP TypeScript upbringing, but the goal is 100% conformance with the protocol itself.

For more information about using the `LanguageClient` / `LanguageServer` on it's own [here](./docs/lsp.md).

## Debug Adapter Protocol
We strive to ensure that we implement all the types, events and requests that are defined by the [Debug Adapter Protocol](https://microsoft.github.io/debug-adapter-protocol/).

For more information about using the `DebugAdapterClient` / `DebugAdapterServer` on it's own [here](./docs/dap.md).


# Status
<!-- badges -->
[![github-release-badge]][github-release]
[![github-license-badge]][github-license]
[![codecov-badge]][codecov]
<!-- badges -->

<!-- history badges -->
| Azure Pipelines | GitHub Actions |
| --------------- | -------------- |
| [![azurepipelines-badge]][azurepipelines] | [![github-badge]][github] |
| [![azurepipelines-history-badge]][azurepipelines-history] | [![github-history-badge]][github] |
<!-- history badges -->

<!-- nuget packages -->
| Package | NuGet |
| ------- | ----- |
| OmniSharp.Extensions.DebugAdapter | [![nuget-version-hefb6om79mfg-badge]![nuget-downloads-hefb6om79mfg-badge]][nuget-hefb6om79mfg] |
| OmniSharp.Extensions.DebugAdapter.Client | [![nuget-version-94qjnkon/cda-badge]![nuget-downloads-94qjnkon/cda-badge]][nuget-94qjnkon/cda] |
| OmniSharp.Extensions.DebugAdapter.Server | [![nuget-version-f/4jrt4grmdg-badge]![nuget-downloads-f/4jrt4grmdg-badge]][nuget-f/4jrt4grmdg] |
| OmniSharp.Extensions.DebugAdapter.Shared | [![nuget-version-2fkn0yzdbhmg-badge]![nuget-downloads-2fkn0yzdbhmg-badge]][nuget-2fkn0yzdbhmg] |
| OmniSharp.Extensions.DebugAdapter.Testing | [![nuget-version-jppuysmkpfcw-badge]![nuget-downloads-jppuysmkpfcw-badge]][nuget-jppuysmkpfcw] |
| OmniSharp.Extensions.JsonRpc | [![nuget-version-a1bmkwyotvkg-badge]![nuget-downloads-a1bmkwyotvkg-badge]][nuget-a1bmkwyotvkg] |
| OmniSharp.Extensions.JsonRpc.Testing | [![nuget-version-punkj7/efvjq-badge]![nuget-downloads-punkj7/efvjq-badge]][nuget-punkj7/efvjq] |
| OmniSharp.Extensions.LanguageClient | [![nuget-version-fclou9t/p2ba-badge]![nuget-downloads-fclou9t/p2ba-badge]][nuget-fclou9t/p2ba] |
| OmniSharp.Extensions.LanguageProtocol | [![nuget-version-vddj9t6jnirq-badge]![nuget-downloads-vddj9t6jnirq-badge]][nuget-vddj9t6jnirq] |
| OmniSharp.Extensions.LanguageProtocol.Testing | [![nuget-version-md8c3c/bo/8g-badge]![nuget-downloads-md8c3c/bo/8g-badge]][nuget-md8c3c/bo/8g] |
| OmniSharp.Extensions.LanguageServer | [![nuget-version-fkxlzvrmzpbw-badge]![nuget-downloads-fkxlzvrmzpbw-badge]][nuget-fkxlzvrmzpbw] |
| OmniSharp.Extensions.LanguageServer.Shared | [![nuget-version-4htmykprzq1a-badge]![nuget-downloads-4htmykprzq1a-badge]][nuget-4htmykprzq1a] |
<!-- nuget packages -->

## License

Copyright © .NET Foundation, and contributors.

OmniSharp is provided as-is under the MIT license. For more information see [LICENSE](https://github.com/OmniSharp/omnisharp-roslyn/blob/master/license.md).

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/)
to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).

## Contribution License Agreement

By signing the [CLA](https://cla.dotnetfoundation.org/OmniSharp/omnisharp-roslyn), the community is free to use your contribution to .NET Foundation projects.

## .NET Foundation

This project is supported by the [.NET Foundation](http://www.dotnetfoundation.org).

<!-- generated references -->
[github-release]: https://github.com/OmniSharp/csharp-language-server-protocol/releases/latest
[github-release-badge]: https://img.shields.io/github/release/OmniSharp/csharp-language-server-protocol.svg?logo=github&style=flat "Latest Release"
[github-license]: https://github.com/OmniSharp/csharp-language-server-protocol/blob/master/LICENSE
[github-license-badge]: https://img.shields.io/github/license/OmniSharp/csharp-language-server-protocol.svg?style=flat "License"
[codecov]: https://codecov.io/gh/OmniSharp/csharp-language-server-protocol
[codecov-badge]: https://img.shields.io/codecov/c/github/OmniSharp/csharp-language-server-protocol.svg?color=E03997&label=codecov&logo=codecov&logoColor=E03997&style=flat "Code Coverage"
[azurepipelines]: https://dev.azure.com/omnisharp/Builds/_build/latest?definitionId=1&branchName=master
[azurepipelines-badge]: https://img.shields.io/azure-devops/build/omnisharp/Builds/1.svg?color=98C6FF&label=azure%20pipelines&logo=azuredevops&logoColor=98C6FF&style=flat "Azure Pipelines Status"
[azurepipelines-history]: https://dev.azure.com/omnisharp/Builds/_build?definitionId=1&branchName=master
[azurepipelines-history-badge]: https://buildstats.info/azurepipelines/chart/omnisharp/Builds/1?includeBuildsFromPullRequest=false "Azure Pipelines History"
[github]: https://github.com/OmniSharp/csharp-language-server-protocol/actions?query=workflow%3Aci
[github-badge]: https://img.shields.io/github/workflow/status/OmniSharp/csharp-language-server-protocol/ci.svg?label=github&logo=github&color=b845fc&logoColor=b845fc&style=flat "GitHub Actions Status"
[github-history-badge]: https://buildstats.info/github/chart/OmniSharp/csharp-language-server-protocol?includeBuildsFromPullRequest=false "GitHub Actions History"
[nuget-hefb6om79mfg]: https://www.nuget.org/packages/OmniSharp.Extensions.DebugAdapter/
[nuget-version-hefb6om79mfg-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.DebugAdapter.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-hefb6om79mfg-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.DebugAdapter.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-94qjnkon/cda]: https://www.nuget.org/packages/OmniSharp.Extensions.DebugAdapter.Client/
[nuget-version-94qjnkon/cda-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.DebugAdapter.Client.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-94qjnkon/cda-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.DebugAdapter.Client.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-f/4jrt4grmdg]: https://www.nuget.org/packages/OmniSharp.Extensions.DebugAdapter.Server/
[nuget-version-f/4jrt4grmdg-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.DebugAdapter.Server.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-f/4jrt4grmdg-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.DebugAdapter.Server.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-2fkn0yzdbhmg]: https://www.nuget.org/packages/OmniSharp.Extensions.DebugAdapter.Shared/
[nuget-version-2fkn0yzdbhmg-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.DebugAdapter.Shared.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-2fkn0yzdbhmg-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.DebugAdapter.Shared.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-jppuysmkpfcw]: https://www.nuget.org/packages/OmniSharp.Extensions.DebugAdapter.Testing/
[nuget-version-jppuysmkpfcw-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.DebugAdapter.Testing.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-jppuysmkpfcw-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.DebugAdapter.Testing.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-a1bmkwyotvkg]: https://www.nuget.org/packages/OmniSharp.Extensions.JsonRpc/
[nuget-version-a1bmkwyotvkg-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.JsonRpc.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-a1bmkwyotvkg-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.JsonRpc.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-punkj7/efvjq]: https://www.nuget.org/packages/OmniSharp.Extensions.JsonRpc.Testing/
[nuget-version-punkj7/efvjq-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.JsonRpc.Testing.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-punkj7/efvjq-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.JsonRpc.Testing.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-fclou9t/p2ba]: https://www.nuget.org/packages/OmniSharp.Extensions.LanguageClient/
[nuget-version-fclou9t/p2ba-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.LanguageClient.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-fclou9t/p2ba-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.LanguageClient.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-vddj9t6jnirq]: https://www.nuget.org/packages/OmniSharp.Extensions.LanguageProtocol/
[nuget-version-vddj9t6jnirq-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.LanguageProtocol.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-vddj9t6jnirq-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.LanguageProtocol.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-md8c3c/bo/8g]: https://www.nuget.org/packages/OmniSharp.Extensions.LanguageProtocol.Testing/
[nuget-version-md8c3c/bo/8g-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.LanguageProtocol.Testing.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-md8c3c/bo/8g-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.LanguageProtocol.Testing.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-fkxlzvrmzpbw]: https://www.nuget.org/packages/OmniSharp.Extensions.LanguageServer/
[nuget-version-fkxlzvrmzpbw-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.LanguageServer.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-fkxlzvrmzpbw-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.LanguageServer.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
[nuget-4htmykprzq1a]: https://www.nuget.org/packages/OmniSharp.Extensions.LanguageServer.Shared/
[nuget-version-4htmykprzq1a-badge]: https://img.shields.io/nuget/v/OmniSharp.Extensions.LanguageServer.Shared.svg?color=004880&logo=nuget&style=flat-square "NuGet Version"
[nuget-downloads-4htmykprzq1a-badge]: https://img.shields.io/nuget/dt/OmniSharp.Extensions.LanguageServer.Shared.svg?color=004880&logo=nuget&style=flat-square "NuGet Downloads"
<!-- generated references -->

<!-- nuke-data
github:
  owner: OmniSharp
  repository: csharp-language-server-protocol
azurepipelines:
  account: omnisharp
  teamproject: Builds
  builddefinition: 1
-->
