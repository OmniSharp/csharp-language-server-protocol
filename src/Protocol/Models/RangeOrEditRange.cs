namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

public partial record RangeOrEditRange
{
    public RangeOrEditRange(Range range)
    {
        Range = range;
        EditRange = null;
    }

    public RangeOrEditRange(EditRange editRange)
    {
        Range = null;
        EditRange = editRange;
    }

    public bool IsRange => Range is not null;
    public Range? Range { get; set; }

    public bool IsEditRange => EditRange != null;
    public EditRange? EditRange { get; }

    public static implicit operator RangeOrEditRange(Range range) => new RangeOrEditRange(range);

    public static implicit operator RangeOrEditRange(EditRange editRange) => new RangeOrEditRange(editRange);


}
