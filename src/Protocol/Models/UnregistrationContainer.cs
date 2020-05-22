using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class UnregistrationContainer : Container<Unregistration>
    {

        public UnregistrationContainer() : this(Enumerable.Empty<Unregistration>()) { }

        public UnregistrationContainer(IEnumerable<Unregistration> items) : base(items) { }

        public UnregistrationContainer(params Unregistration[] items) : base(items) { }


        public static implicit operator UnregistrationContainer(Unregistration[] items)
        {
            return new UnregistrationContainer(items);
        }

        public static implicit operator UnregistrationContainer(Collection<Unregistration> items)
        {
            return new UnregistrationContainer(items);
        }

        public static implicit operator UnregistrationContainer(List<Unregistration> items)
        {
            return new UnregistrationContainer(items);
        }


        public static implicit operator UnregistrationContainer(Registration[] items)
        {
            return new UnregistrationContainer(items.Select(x => (Unregistration) x));
        }

        public static implicit operator UnregistrationContainer(Collection<Registration> items)
        {
            return new UnregistrationContainer(items.Select(x => (Unregistration)x));
        }

        public static implicit operator UnregistrationContainer(List<Registration> items)
        {
            return new UnregistrationContainer(items.Select(x => (Unregistration)x));
        }
    }
}
