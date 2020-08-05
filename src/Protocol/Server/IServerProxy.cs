using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface IServerProxy : IResponseRouter
    {
        IProgressManager ProgressManager { get; }
        IServerWorkDoneManager WorkDoneManager { get; }
        ILanguageServerConfiguration Configuration { get; }
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }
    }
}
