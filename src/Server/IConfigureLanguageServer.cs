namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface IConfigureLanguageServer
    {
        void Configure(LanguageServerOptions options);
    }
}
