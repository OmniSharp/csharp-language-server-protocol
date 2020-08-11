using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class RegistrationContainer : Container<Registration>
    {
        public RegistrationContainer() : this(Enumerable.Empty<Registration>())
        {
        }

        public RegistrationContainer(IEnumerable<Registration> items) : base(items)
        {
        }

        public RegistrationContainer(params Registration[] items) : base(items)
        {
        }


        public static implicit operator RegistrationContainer(Registration[] items) => new RegistrationContainer(items);

        public static implicit operator RegistrationContainer(Collection<Registration> items) => new RegistrationContainer(items);

        public static implicit operator RegistrationContainer(List<Registration> items) => new RegistrationContainer(items);
    }
}
