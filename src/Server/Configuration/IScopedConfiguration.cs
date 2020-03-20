using System;
using Microsoft.Extensions.Configuration;

namespace OmniSharp.Extensions.LanguageServer.Server.Configuration
{
    public interface IScopedConfiguration : IDisposable, IConfiguration { }
}
