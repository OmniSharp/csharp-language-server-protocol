namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public partial record Position
    {
        /// <summary>
        /// Derive a new position from this position.
        /// </summary>
        public Position Delta(int deltaLine = 0, int deltaCharacter = 0)
        {
            return this with { Line = Line + deltaLine, Character = Character + deltaCharacter };
        }
    }
}
