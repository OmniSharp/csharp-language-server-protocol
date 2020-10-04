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

        public static implicit operator CommandOrCodeActionContainer(CommandOrCodeAction[] items) => new CommandOrCodeActionContainer(items);

        public static implicit operator CommandOrCodeActionContainer(Collection<CommandOrCodeAction> items) => new CommandOrCodeActionContainer(items);

        public static implicit operator CommandOrCodeActionContainer(List<CommandOrCodeAction> items) => new CommandOrCodeActionContainer(items);
    }

    /// <remarks>
    /// Typed code lens used for the typed handlers
    /// </remarks>
    public class CodeActionContainer<T> : Container<CodeAction<T>> where T : HandlerIdentity?, new()
    {
        public CodeActionContainer() : this(Enumerable.Empty<CodeAction<T>>())
        {
        }

        public CodeActionContainer(IEnumerable<CodeAction<T>> items) : base(items)
        {
        }

        public CodeActionContainer(params CodeAction<T>[] items) : base(items)
        {
        }

        public static implicit operator CodeActionContainer<T>(CodeAction<T>[] items) => new CodeActionContainer<T>(items);

        public static implicit operator CodeActionContainer<T>(Collection<CodeAction<T>> items) => new CodeActionContainer<T>(items);

        public static implicit operator CodeActionContainer<T>(List<CodeAction<T>> items) => new CodeActionContainer<T>(items);

        public static implicit operator CommandOrCodeActionContainer(CodeActionContainer<T> container) => new CommandOrCodeActionContainer(container.Select(z => new CommandOrCodeAction(z)));
    }
}
