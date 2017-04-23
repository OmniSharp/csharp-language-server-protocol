using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
