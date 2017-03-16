using System.Collections.Generic;

namespace Lsp.Models
{
    public class SignatureHelpRegistrationOptions : TextDocumentRegistrationOptions
    {
        /// <summary>
        /// The characters that trigger signature help
        /// automatically.
        /// </summary>
        public Container<string> TriggerCharacters { get; set; }
    }
}