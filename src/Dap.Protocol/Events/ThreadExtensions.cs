using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class ThreadExtensions
    {
        public static void SendThread(this IDebugClient mediator, ThreadEvent @event)
        {
            mediator.SendNotification(EventNames.Thread, @event);
        }
    }
}
