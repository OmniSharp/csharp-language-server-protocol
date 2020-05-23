using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IInputHandler
    {
        void Start();
        Task StopAsync();
    }
}
