using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class LoadedSourceExtensions
    {
        public static void SendLoadedSource(this IDebugClient mediator, LoadedSourceEvent @event)
        {
            mediator.SendNotification(EventNames.LoadedSource, @event);
        }
    }
}
