namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

public partial record EditRange
{
    public Range Insert { get; init; }
    public Range Replace { get; init; }
}