using System.Collections.Generic;
using System.Linq;

namespace Lsp.Models
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
    }
}