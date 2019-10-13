using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class CommandOrCodeActionContainer : Container<CommandOrCodeAction>
    {
        public CommandOrCodeActionContainer() : this(Enumerable.Empty<CommandOrCodeAction>())
        {
        }

        public CommandOrCodeActionContainer(IEnumerable<CommandOrCodeAction> items) : base(items)
        {
        }

        public CommandOrCodeActionContainer(params CommandOrCodeAction[] items) : base(items)
        {
        }

        public static implicit operator CommandOrCodeActionContainer(CommandOrCodeAction[] items)
        {
            return new CommandOrCodeActionContainer(items);
        }

        public static implicit operator CommandOrCodeActionContainer(Collection<CommandOrCodeAction> items)
        {
            return new CommandOrCodeActionContainer(items);
        }

        public static implicit operator CommandOrCodeActionContainer(List<CommandOrCodeAction> items)
        {
            return new CommandOrCodeActionContainer(items);
        }
    }
}
