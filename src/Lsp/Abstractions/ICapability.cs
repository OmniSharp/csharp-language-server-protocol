namespace Lsp
{
    public interface ICapability<TCapability>
    {
        void SetCapability(TCapability capability);
    }
}