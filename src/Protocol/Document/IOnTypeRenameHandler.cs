using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel]
    [Obsolete(Constants.Proposal)]
    [Method(TextDocumentNames.OnTypeRename, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IOnTypeRenameHandler : IJsonRpcRequestHandler<OnTypeRenameParams, OnTypeRenameRanges?>, IRegistration<OnTypeRenameRegistrationOptions, OnTypeRenameClientCapabilities>
    {
    }
}
