using System;
using Microsoft.Extensions.Configuration;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface IScopedConfiguration : IDisposable, IConfiguration { }
}
