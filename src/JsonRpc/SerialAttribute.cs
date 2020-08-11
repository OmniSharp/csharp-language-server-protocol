namespace OmniSharp.Extensions.JsonRpc
{
    public sealed class SerialAttribute : ProcessAttribute
    {
        public SerialAttribute() : base(RequestProcessType.Serial)
        {
        }
    }
}
