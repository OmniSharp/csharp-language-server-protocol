using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Jobs;

namespace Pipeline
{
    [MemoryDiagnoser]
    // [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.NetCoreApp21)]
    public class ClassicVsPipelines
    {
        private const string sampleCommand =
            "Content-Length: 88\r\n\r\n{\"seq\":1,\"type\":\"response\",\"request_seq\":1,\"success\":true,\"command\":\"command\",\"body\":{}}";

        private const string anotherPayload =
            "Content-Length: 894\r\n\r\n{\"edit\":{\"documentChanges\":[{\"textDocument\":{\"version\":1,\"uri\":\"file:///abc/123/d.cs\"},\"edits\":[{\"range\":{\"start\":{\"line\":1,\"character\":1},\"end\":{\"line\":2,\"character\":2}},\"newText\":\"new text\"},{\"range\":{\"start\":{\"line\":3,\"character\":3},\"end\":{\"line\":4,\"character\":4}},\"newText\":\"new text2\"}]},{\"textDocument\":{\"version\":1,\"uri\":\"file:///abc/123/b.cs\"},\"edits\":[{\"range\":{\"start\":{\"line\":1,\"character\":1},\"end\":{\"line\":2,\"character\":2}},\"newText\":\"new text2\"},{\"range\":{\"start\":{\"line\":3,\"character\":3},\"end\":{\"line\":4,\"character\":4}},\"newText\":\"new text3\"}]},{\"kind\":\"create\",\"uri\":\"file:///abc/123/b.cs\",\"options\":{\"overwrite\":true,\"ignoreIfExists\":true}},{\"kind\":\"rename\",\"oldUri\":\"file:///abc/123/b.cs\",\"newUri\":\"file:///abc/123/c.cs\",\"options\":{\"overwrite\":true,\"ignoreIfExists\":true}},{\"kind\":\"delete\",\"uri\":\"file:///abc/123/c.cs\",\"options\":{\"recursive\":false,\"ignoreIfNotExists\":true}}]}}";


        private ClassicHandler _classic;
        private PipelinesBased _pipelines;

        public ClassicVsPipelines()
        {
        }

        [Params(
            sampleCommand,
            anotherPayload
        )]
        public string Payload { get; set; }

        [Params(
            // 10,
            100,
            1000
        )]
        public int Count { get; set; }

        public byte[] Bytes { get; set; }

        [GlobalSetup]
        public void SetupPipelines()
        {
            var bytes = Encoding.ASCII.GetBytes(Payload);
            Bytes = Enumerable.Range(0, Count).SelectMany(z => bytes).ToArray();
        }

        [Benchmark]
        public Task Classic()
        {
            var pipe = new Pipe();
            pipe.Writer.Write(Bytes);
            pipe.Writer.Complete();
            _classic = new ClassicHandler(pipe.Reader.AsStream());
            return _classic.ProcessInputStream();
        }

        [Benchmark]
        public Task Pipelines()
        {
            var pipe = new Pipe();
            pipe.Writer.Write(Bytes);
            pipe.Writer.Complete();
            _pipelines = new PipelinesBased(pipe.Reader);
            return _pipelines.ProcessInputStream(CancellationToken.None);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ClassicVsPipelines>();
        }
    }
}
