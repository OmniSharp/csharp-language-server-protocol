using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    /// <summary>
    /// A simple marker interface to use for storing handlings (which will be cast out later)
    /// </summary>
    public interface IJsonRpcHandler { }
}
