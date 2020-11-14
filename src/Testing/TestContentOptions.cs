namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    public class TestContentOptions
    {
        public PositionMarker PositionMarker { get; set; } = ( '$', '$' );
        public (PositionMarker open, char labelStop, PositionMarker close) NamedRangeMarker { get; set; } = ( ('{', '|'), ':', ('|', '}') );
        public (PositionMarker open, PositionMarker close) RangeMarker { get; set; } = ( ('[', '|'), ('|', ']') );
    }
}