using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class PrepareRenameParams : TextDocumentPositionParams, IRequest<RangeOrPlaceholderRange>
    {
    }
}
