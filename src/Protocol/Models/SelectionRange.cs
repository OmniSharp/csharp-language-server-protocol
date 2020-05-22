namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class SelectionRange
    {
        /// <summary>
        /// The [range](#Range) of this selection range.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The parent selection range containing this range. Therefore `parent.range` must contain `this.range`.
        /// </summary>
        public SelectionRange Parent { get; set; }
    }
}
