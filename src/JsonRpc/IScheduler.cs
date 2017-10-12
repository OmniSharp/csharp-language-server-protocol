using System;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IScheduler : IDisposable
    {
        void Start();
        void Add(RequestProcessType type, string name, Func<Task> request);
    }
}
