using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests
{
    public class PipeTests
        : TestBase
    {
        public PipeTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
        }
    }
}
