﻿using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    /// <summary>
    /// Gives your class or handler an opportunity to interact with
    /// the <see cref="ILanguageServer"/> after the connection has been established.
    /// </summary>
    public delegate Task OnLanguageServerStartedDelegate(ILanguageServer server, CancellationToken cancellationToken);
}
