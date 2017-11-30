using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CommandContainer : Container<Command>
    {
        public CommandContainer() : this(Enumerable.Empty<Command>())
        {
        }

        public CommandContainer(IEnumerable<Command> items) : base(items)
        {
        }

        public CommandContainer(params Command[] items) : base(items)
        {
        }

        public static implicit operator CommandContainer(Command[] items)
        {
            return new CommandContainer(items);
        }

        public static implicit operator CommandContainer(Collection<Command> items)
        {
            return new CommandContainer(items);
        }

        public static implicit operator CommandContainer(List<Command> items)
        {
            return new CommandContainer(items);
        }
    }
}