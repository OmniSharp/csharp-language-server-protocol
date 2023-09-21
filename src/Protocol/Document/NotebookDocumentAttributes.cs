namespace OmniSharp.Extensions.LanguageServer.Protocol.Document;

public class NotebookDocumentAttributes : IEquatable<NotebookDocumentAttributes>
{
    public NotebookDocumentAttributes(DocumentUri uri, string notebookType)
    {
        Uri = uri;
        NotebookType = notebookType;
    }

    public NotebookDocumentAttributes(DocumentUri uri)
    {
        Uri = uri;
    }

    public NotebookDocumentAttributes(string language)
    {
        Language = language;
    }

    public NotebookDocumentAttributes(string language, DocumentUri uri)
    {
        Language = language;
        Uri = uri;
    }

    public NotebookDocumentAttributes(DocumentUri uri, string scheme, string notebookType)
    {
        Uri = uri;
        Scheme = scheme;
        NotebookType = notebookType;
    }

    public DocumentUri Uri { get; }
    public string? Scheme { get; }
    public string? NotebookType { get; }
    public string? Language { get; }

    public bool Equals(NotebookDocumentAttributes? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Uri.Equals(other.Uri) && Scheme == other.Scheme && NotebookType == other.NotebookType;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((NotebookDocumentAttributes) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Uri.GetHashCode();
            hashCode = ( hashCode * 397 ) ^ ( Scheme != null ? Scheme.GetHashCode() : 0 );
            hashCode = ( hashCode * 397 ) ^ ( NotebookType != null ? NotebookType.GetHashCode() : 0 );
            hashCode = ( hashCode * 397 ) ^ ( Language != null ? Language.GetHashCode() : 0 );
            return hashCode;
        }
    }

    public static bool operator ==(NotebookDocumentAttributes? left, NotebookDocumentAttributes? right) => Equals(left, right);

    public static bool operator !=(NotebookDocumentAttributes? left, NotebookDocumentAttributes? right) => !Equals(left, right);
}
