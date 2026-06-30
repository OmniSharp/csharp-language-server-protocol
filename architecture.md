# Architecture: Language Server Protocol implementation

This repository implements the Language Server Protocol (LSP) as a set of layered .NET packages. The lower layers provide a bidirectional JSON-RPC runtime; the LSP layers add protocol models, serializers, request routing, client/server startup, capability negotiation, and dynamic registration.

## Package layout

| Area | Project | Role |
| --- | --- | --- |
| JSON-RPC runtime | `src\JsonRpc` | Transport framing, request/response dispatch, handler registration, request scheduling, cancellation, serialization primitives, and DI integration. |
| LSP protocol model | `src\Protocol` | LSP request/notification models, client and server capabilities, registration options, typed protocol proxy interfaces, serializers, converters, and handler base classes. |
| Shared LSP runtime | `src\Shared` | LSP-specific handler collection, descriptor creation, descriptor matching, routing, supported capability tracking, progress support, and common DI registrations. |
| Server facade | `src\Server` | `LanguageServer`, server lifecycle, initialization handling, server capability calculation, dynamic registration, configuration, workspace folders, and server-side proxies. |
| Client facade | `src\Client` | `LanguageClient`, client lifecycle, initialize request creation, client capability construction, static/dynamic registration tracking, and client-side proxies. |
| Source generators | `src\JsonRpc.Generators` | Compile-time generation for handler interfaces, handler base classes, delegate registration methods, protocol proxy request methods, container helpers, and enum-like strings. |
| Proposals | `src\Protocol.Proposals` | Protocol additions that are not yet stable in the base LSP package. |
| Testing support | `src\Testing` and `src\JsonRpc.Testing` | In-memory protocol test harnesses and request settling utilities. |

The Debug Adapter Protocol projects follow the same broad architecture, but this document focuses on LSP.

## Layering model

The implementation is intentionally split so the JSON-RPC runtime is protocol-agnostic:

```text
Streams / PipeReader / PipeWriter
        |
        v
src\JsonRpc
  Connection -> InputHandler / OutputHandler
  Receiver -> RequestRouter -> RequestInvoker -> handlers
  ResponseRouter <- outgoing requests
        |
        v
src\Protocol + src\Shared
  LSP models, LspSerializer, LspRequestRouter,
  LspHandlerDescriptor, capabilities, registration options
        |
        v
src\Server / src\Client
  LanguageServer and LanguageClient lifecycle,
  initialize handshake, facades, work done/progress,
  dynamic registration and configuration helpers
```

LSP features are represented as typed request or notification records in `src\Protocol\Features`. Those records carry protocol metadata through attributes such as `[Method]`, `[Parallel]`, `[Serial]`, `[RegistrationOptions]`, `[Capability]`, and `[Resolver]`. Source generators consume that metadata to produce the public handler and proxy APIs.

For example, `CompletionParams` in `src\Protocol\Features\Document\CompletionFeature.cs` is a request model for `textDocument/completion`. Its attributes define the JSON-RPC method name, direction, request scheduling behavior, registration option type, client capability type, and generated APIs. The generator turns that model into handler interfaces/base classes and convenience methods on language client/server protocol proxies.

## JSON-RPC transport and dispatch

`src\JsonRpc` handles the wire-level JSON-RPC mechanics used by LSP:

1. `Connection` owns an `InputHandler` and opens the input loop.
2. `InputHandler` reads LSP-style `Content-Length: ...\r\n\r\n` framed messages from a `PipeReader`, parses the JSON payload with Newtonsoft.Json, validates it through an `IReceiver`, and separates requests, notifications, responses, and errors.
3. Incoming responses are completed through `IResponseRouter`, which maps response IDs back to pending outgoing requests.
4. Incoming requests and notifications are routed through `IRequestRouter`.
5. `RequestInvoker` invokes the selected handler descriptors, applies serial/parallel scheduling, request timeouts, cancellation, and error mapping.
6. `OutputHandler` serializes outgoing messages, writes JSON-RPC framing to a `PipeWriter`, and can delay messages until output filters allow them.

The JSON-RPC layer knows only about JSON-RPC handlers and method names. It does not know about document selectors, LSP capabilities, dynamic registration, or LSP-specific union types; those are added by the LSP shared layer.

## LSP models and serialization

`src\Protocol` maps LSP specification concepts into .NET types:

- Request and notification parameter records implement MediatR request shapes such as `IRequest<TResult>` for requests and `IRequest` for notifications.
- Protocol models live mostly under `src\Protocol\Models` and feature files under `src\Protocol\Features`.
- Client capabilities are modeled under `src\Protocol\Client\Capabilities`.
- Server capabilities are modeled under `src\Protocol\Server\Capabilities`.
- Registration options and capability interfaces describe how features participate in static and dynamic registration.
- Helper types such as `Container<T>`, `BooleanOr<T>`, `DocumentUri`, `Position`, and `Range` smooth over TypeScript-oriented LSP shapes in idiomatic C#.

`LspSerializer` in `src\Protocol\Serialization\Serializer.cs` extends the JSON-RPC serializer with LSP-specific converters and contract resolution. It handles protocol union shapes, optional values, string/number enums, `DocumentUri`, progress tokens, completion lists, markup content, semantic token shapes, workspace edits, and other LSP-specific wire formats.

The serializer is capability-aware. During initialization, the client and server feed negotiated capabilities into the serializer so it can constrain serialized enum-like values to what the peer supports, such as completion item kinds, symbol kinds, diagnostic tags, code action kinds, and semantic token types.

## Handler model

Handlers are normal .NET objects implementing generated or generic JSON-RPC interfaces:

- `IJsonRpcNotificationHandler<TParams>`
- `IJsonRpcRequestHandler<TParams>`
- `IJsonRpcRequestHandler<TParams, TResult>`
- generated LSP-specific interfaces such as completion, hover, or initialize handlers
- abstract base classes in `AbstractHandlers` for handlers that need registration options, capabilities, partial results, or initial values

Handlers can be registered as instances, types, factories, or delegates. The registry APIs are exposed by options objects at construction time and by runtime `Register(...)` methods on `LanguageServer`, `LanguageClient`, and the underlying JSON-RPC server.

`SharedHandlerCollection` is the central LSP handler registry. When a handler is added, it:

- inspects the handler's implemented interfaces,
- asks `LspHandlerTypeDescriptorProvider` for method/capability/registration metadata,
- creates `LspHandlerDescriptor` entries,
- tracks text document identifiers where applicable,
- infers request process type from explicit options or `[Serial]` / `[Parallel]`,
- assigns stable routing keys after initialization based on registration options, document selectors, execute-command names, and handler IDs.

After the collection is initialized, descriptors contain enough information for both request routing and capability registration.

## LSP request routing

`LspRequestRouter` in `src\Shared` specializes JSON-RPC routing for LSP. It first finds descriptors by JSON-RPC method name, deserializes the incoming params to the descriptor's parameter type, and then applies registered `IHandlerMatcher` strategies.

This enables LSP-specific routing behavior:

- text document requests can be routed by `DocumentSelector`,
- execute-command requests can be routed by command name,
- resolve requests can be sent back to the handler that created the original item,
- multiple handlers for one method can be selected when the protocol feature supports aggregation.

If no LSP matcher selects a descriptor, the router falls back to all descriptors for the method. Execute-command is stricter: if no command-specific handler matches, the route is treated as missing.

## Client and server lifecycle

Both `LanguageServer` and `LanguageClient` derive from `JsonRpcServerBase`, because JSON-RPC is bidirectional: either peer can send requests and receive requests.

### Server startup

`LanguageServer` in `src\Server\LanguageServer.cs` is created through `LanguageServer.Create(...)`, `LanguageServer.From(...)`, or DI registration. Its container is built from `JsonRpcServerContainer` plus LSP/server internals.

At initialization:

1. The server opens the JSON-RPC connection and waits for `initialize`.
2. The generated internal initialize handler receives raw client capability JSON.
3. The server deserializes client capabilities, records the client LSP version, initializes `SupportedCapabilities`, and updates `LspSerializer`.
4. Server initialize callbacks run.
5. The handler collection is initialized, producing final descriptor keys and registration options.
6. Server capabilities are computed from registered handlers and registration-option converters.
7. Initialized callbacks run.
8. The receiver is marked initialized so normal incoming messages can be processed.
9. For LSP 3 clients, the server completes startup after receiving `initialized`.
10. Started callbacks run and the server is marked started.

Static capabilities are returned in the `InitializeResult`. Dynamic registrations are delayed until initialization is complete.

### Client startup

`LanguageClient` in `src\Client\LanguageClient.cs` follows the complementary flow:

1. It builds `InitializeParams` from client options, root URI, workspace folders, initialization options, trace settings, and client capabilities.
2. Capability instances are merged into `ClientCapabilities`.
3. The handler collection is initialized.
4. Client initialize callbacks run.
5. The JSON-RPC connection opens.
6. The client sends `initialize` to the server and stores the returned `InitializeResult`.
7. The serializer is updated with server capabilities.
8. Initialized callbacks run.
9. The receiver is marked initialized.
10. Static server capabilities are registered with the client registration manager.
11. The client sends the `initialized` notification.
12. Started callbacks run and the client is marked started.

## Capabilities and registration

LSP capability negotiation is split across three concepts:

- **Client capabilities** describe what the peer can handle. They are sent in `initialize`.
- **Server capabilities** describe statically available server features. They are returned in `InitializeResult`.
- **Registration options** describe per-feature routing and behavior, such as document selectors, trigger characters, command names, and work-done progress.

Feature model types declare their capability and registration option types with attributes. Registration-option converters know how to turn server registration options into the corresponding `ServerCapabilities` fields for static registration.

The server avoids mixing static and dynamic registration for the same feature. When a client supports dynamic registration for a capability, server handlers for that capability are not emitted into `ServerCapabilities`; instead `LanguageServerHelpers.DynamicallyRegisterHandlers(...)` sends `client/registerCapability` after initialization. When dynamic registration is not supported, registration options are converted into static server capability properties.

Dynamic registrations are disposable. When a dynamically added handler registration is disposed, the server sends `client/unregisterCapability` for the corresponding registration IDs.

## Source generation

`src\JsonRpc.Generators` removes most of the repetitive protocol plumbing. Attributes on protocol models drive generated code:

- `[GenerateHandler]` creates typed handler interfaces and abstract base classes.
- `[GenerateHandlerMethods]` creates delegate-based registration helpers such as `OnCompletion(...)`.
- `[GenerateRequestMethods]` creates strongly typed proxy methods such as `RequestCompletion(...)` or notification send helpers.
- LSP-specific attributes add capability, registration, and resolver metadata.
- Additional generators create container helpers, typed data helpers, registration option glue, and enum-like string support.

This means adding or updating an LSP feature usually starts by defining the protocol model and annotating it correctly. The public handler and proxy APIs are then produced consistently at compile time.

## Dependency injection and composition

The runtime uses DryIoc internally while exposing Microsoft.Extensions.DependencyInjection-style APIs. Each facade has two creation modes:

- Standalone factory methods such as `LanguageServer.Create(options => { ... })`.
- Service collection extensions such as `services.AddLanguageServer(...)` and `services.AddLanguageClient(...)`.

`LanguageProtocolServiceCollectionExtensions.AddLanguageProtocolInternals(...)` wires the shared LSP services:

- JSON-RPC core services,
- `LspSerializer`,
- `SupportedCapabilities`,
- `TextDocumentIdentifiers`,
- `LspRequestRouter`,
- `SharedHandlerCollection`,
- `ResponseRouter`,
- `ProgressManager`,
- registration option converters discovered from assemblies.

Options can add assemblies for scanning. Assembly scanning is used to find generated metadata, capability keys, handler descriptors, and registration option converters, especially for custom protocol extensions.

## Protocol facades and proxies

`LanguageServer` and `LanguageClient` expose grouped protocol facades for LSP domains:

- `TextDocument`
- `NotebookDocument`
- `Workspace`
- `Window`
- `General`
- `Client`

These facades are strongly typed proxy surfaces over the underlying JSON-RPC response router. Generated request methods use the proxy interfaces to send protocol requests and notifications with the correct method names and result types.

Handlers should not directly depend on `ILanguageServer` or `ILanguageClient`, because handlers are resolved during initialization. They can instead depend on facade interfaces such as `ILanguageServerFacade` or `ILanguageClientFacade`, which expose the relevant protocol surface safely.

## Progress, work done, and partial results

The protocol layer includes abstractions for LSP progress patterns:

- work-done progress support through server/client work-done managers,
- partial result handling for requests that stream results,
- handler base classes for partial results and partial results with initial values,
- serializer support for progress tokens.

Features declare progress participation by implementing interfaces such as `IWorkDoneProgressParams`, `IPartialItemsRequest`, `IPartialItemRequest`, or related generated/auto-implemented interfaces. The source generators and abstract handler bases turn those protocol shapes into handler APIs.

## Configuration and workspace support

The server wraps `workspace/didChangeConfiguration` and `workspace/configuration` into `ILanguageServerConfiguration`, which also implements `Microsoft.Extensions.Configuration.IConfiguration`. Server options can track configuration sections and items, query stateless configuration, and create scoped document configuration views.

Workspace folders are handled by client and server workspace folder managers. During client initialization, the current workspace folders are included in `InitializeParams`; after startup, workspace folder capability and notifications are routed through the normal handler infrastructure.

## Extensibility

Custom protocol extensions use the same machinery as built-in LSP features:

1. Define request or notification parameter models.
2. Add `[Method]`, direction, process type, and generator attributes.
3. Add capability and registration option types when needed.
4. Provide registration option converters for static server capabilities.
5. Include the assembly through options so descriptor and converter scanning can find the extension.

Because the transport, serializer, handler registry, and routers are shared, custom methods can participate in normal request dispatch, dynamic registration, capability negotiation, and generated proxy APIs.
