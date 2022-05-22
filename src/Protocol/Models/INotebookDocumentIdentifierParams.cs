namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

public interface INotebookDocumentIdentifierParams
{
    /// <summary>
    /// The notebook document that got closed.
    /// </summary>
    public NotebookDocumentIdentifier NotebookDocument {get; set;}
}
