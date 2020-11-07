namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// How whitespace and indentation is handled during completion
    /// item insertion.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    public enum InsertTextMode
    {
        /// <summary>
        /// The insertion or replace strings is taken as it is. If the
        /// value is multi line the lines below the cursor will be
        /// inserted using the indentation defined in the string value.
        /// The client will not apply any kind of adjustments to the
        /// string.
        /// </summary>
        AsIs = 1,

        /// <summary>
        /// The editor adjusts leading whitespace of new lines so that
        /// they match the indentation of the line for which the item
        /// is accepted.
        ///
        /// For example if the line containing the cursor when a accepting
        /// a multi line completion item is indented using 3 tabs all
        /// following lines inserted will be indented using 3 tabs as well.
        /// </summary>
        AdjustIndentation = 2
    }
}