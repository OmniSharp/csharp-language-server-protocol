using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JsonRPC.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonRPC
{
    public class Connection : IDisposable
    {
        private readonly TextReader _input;

        private readonly TextWriter _output;

        private readonly Reciever _reciever;
        private readonly Responder _responder;
        private InputHandler _inputHandler;
        private Mediator _mediator;

        public Connection(TextReader input, TextWriter output)
        {
            _input = input;
            _output = output;
            _reciever = new Reciever();
            _responder = new Responder();
            _mediator = new Mediator(new HandlerResolver(AppDomain.CurrentDomain.GetAssemblies()), null);
        }

        private async Task HandleRequest(string request)
        {
            JToken payload;
            try
            {
                payload = JToken.Parse(request);
            }
            catch
            {
                _responder.Respond(new ParseError());
                return;
            }

            if (!_reciever.IsValid(payload))
            {
                _responder.Respond(new InvalidRequest());
                return;
            }

            var requests = _reciever.GetRequests(payload);
            await RespondTo(requests);
        }

        private async Task RespondTo(IEnumerable<ErrorNotificationRequest> items)
        {
            var response = new List<Task<ErrorResponse>>();
            foreach (var item in items)
            {
                if (item.IsRequest)
                {
                    response.Add(_mediator.HandleRequest(item.Request));
                }
                else if (item.IsNotification)
                {
                    _mediator.HandleNotification(item.Notification);
                }
                else
                {
                    response.Add(Task.FromResult<ErrorResponse>(item.Error));
                }
            }

            var result = await Task.WhenAll(response.ToArray());

            if (result.Length == 1)
            {

            }
            else
            {

            }
        }

        public void Open()
        {
            _inputHandler = new InputHandler(_input, (payload) => HandleRequest(payload));
        }

        public void Dispose()
        {
            _inputHandler?.Dispose();
        }
    }
}