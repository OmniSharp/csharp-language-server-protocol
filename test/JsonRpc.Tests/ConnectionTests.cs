using System.IO;
using NSubstitute;

namespace JsonRpc.Tests
{
    public class ConnectionTests
    {
        public void Test()
        {
            var streamIn = Substitute.For<TextReader>();
            var streamOut = Substitute.For<TextWriter>();

            //var connection = new Connection(
            //    streamIn,
            //    streamOut,
            //    new SerialRequestProcessIdentifier()
            //);
        }
    }
}
