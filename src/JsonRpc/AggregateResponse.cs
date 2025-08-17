using System.Collections;

namespace OmniSharp.Extensions.JsonRpc
{
    public class AggregateResponse<T>: IEnumerable where T : IEnumerable
    {
        public IEnumerable<T> Items { get; }

        public AggregateResponse(IEnumerable<T> items) => Items = items.ToArray();

        public AggregateResponse(IEnumerable<object> items) => Items = items.OfType<T>().ToArray();

        public IEnumerator GetEnumerator()
        {
            foreach (var item in Items)
            {
                foreach (var v in item)
                {
                    yield return v;
                }
            }
        }
    }
}
