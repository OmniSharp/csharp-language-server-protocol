using System.Collections.Generic;
using System.Text.Json.Serialization;
using Minimatch;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentFilter
    {
        /// <summary>
        /// A language id, like `typescript`.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public string Language { get; set; }

        /// <summary>
        /// does the document filter contains a language
        /// </summary>
        [JsonIgnore]
        public bool HasLanguage => Language != null;

        /// <summary>
        /// A Uri [scheme](#Uri.scheme), like `file` or `untitled`.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public string Scheme { get; set; }

        /// <summary>
        /// does the document filter contains a scheme
        /// </summary>
        [JsonIgnore]
        public bool HasScheme => Scheme != null;

        /// <summary>
        /// A glob pattern, like `*.{ts,js}`.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public string Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                _minimatcher = new Minimatcher(value, new Options() { MatchBase = true });
            }
        }

        /// <summary>
        /// does the document filter contains a paattern
        /// </summary>
        [JsonIgnore]
        public bool HasPattern => Pattern != null;

        private string _pattern;
        private Minimatcher _minimatcher;

        public static explicit operator string(DocumentFilter documentFilter)
        {
            var items = new List<string>();
            if (documentFilter.HasLanguage)
            {
                items.Add(documentFilter.Language);
            }
            if (documentFilter.HasScheme)
            {
                items.Add(documentFilter.Scheme);
            }
            if (documentFilter.HasPattern)
            {
                items.Add(documentFilter.Pattern);
            }
            return $"[{string.Join(", ", items)}]";
        }

        public bool IsMatch(TextDocumentAttributes attributes)
        {
            if (HasLanguage && HasPattern && HasScheme)
            {
                return Language == attributes.LanguageId && Scheme == attributes.Scheme && _minimatcher.IsMatch(attributes.Uri.ToString());
            }
            if (HasLanguage && HasPattern)
            {
                return Language == attributes.LanguageId && _minimatcher.IsMatch(attributes.Uri.ToString());
            }
            if (HasLanguage && HasScheme)
            {
                return Language == attributes.LanguageId && Scheme == attributes.Scheme;
            }
            if (HasPattern && HasScheme)
            {
                return Scheme == attributes.Scheme && _minimatcher.IsMatch(attributes.Uri.ToString());
            }
            if (HasLanguage)
            {
                return Language == attributes.LanguageId;
            }
            if (HasScheme)
            {
                return Scheme == attributes.Scheme;
            }
            if (HasPattern)
            {
                return _minimatcher.IsMatch(attributes.Uri.ToString());
            }

            return false;
        }

        public static DocumentFilter ForPattern(string wildcard)
        {
            return new DocumentFilter() { Pattern = wildcard };
        }

        public static DocumentFilter ForLanguage(string language)
        {
            return new DocumentFilter() { Language = language };
        }

        public static DocumentFilter ForScheme(string scheme)
        {
            return new DocumentFilter() { Scheme = scheme };
        }
    }
}
