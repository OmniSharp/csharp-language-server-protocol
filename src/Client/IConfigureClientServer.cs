namespace OmniSharp.Extensions.LanguageServer.Client
{
    public interface IConfigureClientServer
    {
        void Configure(LanguageClientOptions options);
    }
}
