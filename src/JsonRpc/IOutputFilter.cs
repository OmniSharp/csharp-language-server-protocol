namespace OmniSharp.Extensions.JsonRpc
{
    /// <summary>
    /// Used to determine if a given message is allowed to be sent before the receiver has been initialized.
    /// </summary>
    public interface IOutputFilter
    {
        bool ShouldOutput(object value);
    }

    class AlwaysOutputFilter : IOutputFilter { public bool ShouldOutput(object value) => true; }
}
