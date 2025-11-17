namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public static class CommandOrCodeActionContainerExtensions
    {
        public static IEnumerable<CodeAction> GetCodeActions(this IEnumerable<CommandOrCodeAction> value ) =>
            value
               .Where(z => z.IsCodeAction)
               .Select(z => z.CodeAction!);

        public static IEnumerable<Command> GetCommands(this IEnumerable<CommandOrCodeAction> value ) =>
            value
               .Where(z => z.IsCommand)
               .Select(z => z.Command!);
    }
}
