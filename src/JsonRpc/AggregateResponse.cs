using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class AggregateResponse<T> where T : IEnumerable
    {
        public IEnumerable<T> Items { get; }

        public AggregateResponse(IEnumerable<T> items)
        {
            Items = items.ToArray();
        }

        public AggregateResponse(IEnumerable<object> items)
        {
            Items = items.OfType<T>().ToArray();
        }
    }
}
