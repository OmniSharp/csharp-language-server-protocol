namespace Lsp
{
    public interface IRegistration<TOptions>
    {
        TOptions GetRegistrationOptions();
    }
}