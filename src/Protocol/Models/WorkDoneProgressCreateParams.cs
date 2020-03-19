using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkDoneProgressCreateParams : IRequest
    {
        /// <summary>
        /// The token to be used to report progress.
        /// </summary>
        public ProgressToken Token { get; set; }
    }
}