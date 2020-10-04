namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Marks that the code action cannot currently be applied.
    ///
    /// Clients should follow the following guidelines regarding disabled code actions:
    ///
    ///   - Disabled code actions are not shown in automatic [lightbulb](https://code.visualstudio.com/docs/editor/editingevolved#_code-action)
    ///     code action menu.
    ///
    ///   - Disabled actions are shown as faded out in the code action menu when the user request a more specific type
    ///     of code action, such as refactorings.
    ///
    ///   - If the user has a [keybinding](https://code.visualstudio.com/docs/editor/refactoring#_keybindings-for-code-actions)
    ///     that auto applies a code action and only a disabled code actions are returned, the client should show the user an
    ///     error message with `reason` in the editor.
    ///
    /// @since 3.16.0
    /// </summary>
    public class CodeActionDisabled
    {
        /// <summary>
        /// Human readable description of why the code action is currently disabled.
        ///
        /// This is displayed in the code actions UI.
        /// </summary>
        public string Reason { get; set; } = null!;
    }
}
