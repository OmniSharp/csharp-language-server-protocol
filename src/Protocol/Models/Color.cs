namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents a color in RGBA space.
    /// </summary>
    public class Color
    {
        /// <summary>
        /// The red component of this color in the range [0-1].
        /// </summary>
        public int Red { get; set; }

        /// <summary>
        /// The green component of this color in the range [0-1].
        /// </summary>
        public int Green { get; set; }

        /// <summary>
        /// The blue component of this color in the range [0-1].
        /// </summary>
        public int Blue { get; set; }

        /// <summary>
        /// The alpha component of this color in the range [0-1].
        /// </summary>
        public int Alpha { get; set; }
    }
}
