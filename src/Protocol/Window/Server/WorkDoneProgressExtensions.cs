using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class WorkDoneProgressExtensions
    {
        public static async Task Create(this ILanguageServerWindowProgress mediator, WorkDoneProgressCreateParams @params)
        {
            await mediator.SendRequest(WindowNames.WorkDoneProgressCreate, @params);
        }
    }
}
