namespace OmniSharp.Extensions.JsonRpc
{
    public sealed class ParallelAttribute : ProcessAttribute
    {
        public ParallelAttribute() : base(RequestProcessType.Parallel)
        {
        }
    }
}
