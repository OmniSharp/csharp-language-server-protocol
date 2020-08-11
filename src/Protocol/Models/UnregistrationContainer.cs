using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class UnregistrationContainer : Container<Unregistration>
    {
        public UnregistrationContainer() : this(Enumerable.Empty<Unregistration>())
        {
        }

        public UnregistrationContainer(IEnumerable<Unregistration> items) : base(items)
        {
        }

        public UnregistrationContainer(params Unregistration[] items) : base(items)
        {
        }


        public static implicit operator UnregistrationContainer(Unregistration[] items) => new UnregistrationContainer(items);

        public static implicit operator UnregistrationContainer(Collection<Unregistration> items) => new UnregistrationContainer(items);

        public static implicit operator UnregistrationContainer(List<Unregistration> items) => new UnregistrationContainer(items);


        public static implicit operator UnregistrationContainer(Registration[] items) => new UnregistrationContainer(items.Select(x => (Unregistration) x));

        public static implicit operator UnregistrationContainer(Collection<Registration> items) => new UnregistrationContainer(items.Select(x => (Unregistration) x));

        public static implicit operator UnregistrationContainer(List<Registration> items) => new UnregistrationContainer(items.Select(x => (Unregistration) x));
    }
}
