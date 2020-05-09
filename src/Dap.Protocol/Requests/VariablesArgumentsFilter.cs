namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VariablesArgumentsFilter
    {
        Indexed, Named
    }

}
