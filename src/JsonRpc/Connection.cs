using System;
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
        private Mediater _mediater;

        public Connection(TextReader input, TextWriter output)
        {
            _input = input;
            _output = output;
            _reciever = new Reciever();
            _responder = new Responder();
            _mediater = new Mediater();
        }

        private async Task HandleRequest(JToken payload)
        {
            if (!_reciever.IsValid(payload))
            {
                _responder.Respond(new InvalidRequest());
                return;
            }

            var requests = _reciever.GetRequests(payload);
            await _mediater.RespondTo(requests);
        }

        public void Open()
        {
            _inputHandler = new InputHandler(_input, async (payload) => await HandleRequest(payload));
        }

        public void Dispose()
        {
            _inputHandler?.Dispose();
        }
    }
}