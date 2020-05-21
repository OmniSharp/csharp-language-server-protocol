using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface IClientProxy : IResponseRouter, IServiceProvider
    {
        IProgressManager ProgressManager { get; }
        IClientWorkDoneManager WorkDoneManager { get; }
        IRegistrationManager RegistrationManager { get; }
        IWorkspaceFoldersManager WorkspaceFoldersManager { get; }
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }
    }
}
