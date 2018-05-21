using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class SignatureHelpParams : TextDocumentPositionParams, IRequest<SignatureHelp> { }
}