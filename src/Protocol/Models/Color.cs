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
        public double Red { get; set; }

        /// <summary>
        /// The green component of this color in the range [0-1].
        /// </summary>
        public double Green { get; set; }

        /// <summary>
        /// The blue component of this color in the range [0-1].
        /// </summary>
        public double Blue { get; set; }

        /// <summary>
        /// The alpha component of this color in the range [0-1].
        /// </summary>
        public double Alpha { get; set; }
    }
}
