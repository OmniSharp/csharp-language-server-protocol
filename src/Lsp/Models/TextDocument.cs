using System.Collections.Generic;

namespace Lsp.Models
{
    public class TextDocument
    {
        public static Container<string> Eol = new string[] { "\n", "\r\n", "\r" };
    }
}