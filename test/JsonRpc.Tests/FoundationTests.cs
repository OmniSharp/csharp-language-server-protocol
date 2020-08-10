using System;
using System.IO.Pipelines;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Pipelines;
using Xunit;
using Unit = MediatR.Unit;

namespace JsonRpc.Tests
{
    public class FoundationTests
    {
        [Theory]
        [ClassData(typeof(CreateData))]
        public void All_Create_From_Methods_Should_Work(ActionDelegate actionDelegate)
        {
            actionDelegate.Method.Should().NotThrow();
        }

        public class CreateData : TheoryData<ActionDelegate>
        {
            public CreateData()
            {
                var baseOptions = new JsonRpcServerOptions().WithPipe(new Pipe());
                void BaseDelegate(JsonRpcServerOptions o) => o.WithPipe(new Pipe());
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                Add(new ActionDelegate("create: options", () => JsonRpcServer.Create(baseOptions)));
                Add(new ActionDelegate("create: options, serviceProvider", () => JsonRpcServer.Create(baseOptions, serviceProvider)));
                Add(new ActionDelegate("create: action", () => JsonRpcServer.Create(BaseDelegate)));
                Add(new ActionDelegate("create: action, serviceProvider", () => JsonRpcServer.Create(BaseDelegate, serviceProvider)));

                Add(new ActionDelegate("from: options", () => JsonRpcServer.From(baseOptions)));
                Add(new ActionDelegate("from: options, cancellationToken", () => JsonRpcServer.From(baseOptions, CancellationToken.None)));
                Add(new ActionDelegate("from: options, serviceProvider, cancellationToken", () => JsonRpcServer.From(baseOptions, serviceProvider, CancellationToken.None)));
                Add(new ActionDelegate("from: options, serviceProvider", () => JsonRpcServer.From(baseOptions, serviceProvider)));
                Add(new ActionDelegate("from: action", () => JsonRpcServer.From(BaseDelegate)));
                Add(new ActionDelegate("from: action, cancellationToken", () => JsonRpcServer.From(BaseDelegate, CancellationToken.None)));
                Add(new ActionDelegate("from: action, serviceProvider, cancellationToken", () => JsonRpcServer.From(BaseDelegate, serviceProvider, CancellationToken.None)));
                Add(new ActionDelegate("from: action, serviceProvider", () => JsonRpcServer.From(BaseDelegate, serviceProvider)));
            }
        }

        [Theory]
        [ClassData(typeof(MissingInputData))]
        public void All_Create_Method_Require_Input(ActionDelegate actionDelegate)
        {
            actionDelegate.Method.Should().Throw<ArgumentException>().Where(z => z.Message.StartsWith("Input is missing!"));
        }

        class MissingInputData : TheoryData<ActionDelegate>
        {
            public MissingInputData()
            {
                var baseOptions = new JsonRpcServerOptions().WithOutput(new Pipe().Writer);
                void BaseDelegate(JsonRpcServerOptions o) => o.WithOutput(new Pipe().Writer);
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                Add(new ActionDelegate("create: options", () => JsonRpcServer.Create(baseOptions)));
                Add(new ActionDelegate("create: options, serviceProvider", () => JsonRpcServer.Create(baseOptions, serviceProvider)));
                Add(new ActionDelegate("create: action", () => JsonRpcServer.Create(BaseDelegate)));
                Add(new ActionDelegate("create: action, serviceProvider", () => JsonRpcServer.Create(BaseDelegate, serviceProvider)));
            }
        }

        [Theory]
        [ClassData(typeof(MissingOutputData))]
        public void All_Create_Method_Require_Output(ActionDelegate actionDelegate)
        {
            actionDelegate.Method.Should().Throw<ArgumentException>().Where(z => z.Message.StartsWith("Output is missing!"));
        }

        class MissingOutputData : TheoryData<ActionDelegate>
        {
            public MissingOutputData()
            {
                var baseOptions = new JsonRpcServerOptions().WithInput(new Pipe().Reader);
                void BaseDelegate(JsonRpcServerOptions o) => o.WithInput(new Pipe().Reader);
                var serviceProvider = new ServiceCollection().BuildServiceProvider();
                Add(new ActionDelegate("create: options", () => JsonRpcServer.Create(baseOptions)));
                Add(new ActionDelegate("create: options, serviceProvider", () => JsonRpcServer.Create(baseOptions, serviceProvider)));
                Add(new ActionDelegate("create: action", () => JsonRpcServer.Create(BaseDelegate)));
                Add(new ActionDelegate("create: action, serviceProvider", () => JsonRpcServer.Create(BaseDelegate, serviceProvider)));
            }
        }

        public class ActionDelegate
        {
            private readonly string _name;
            public Action Method { get; }

            public ActionDelegate(string name, Action method)
            {
                _name = name;
                this.Method = method;
            }

            public override string ToString() => _name;
        }
    }
}
