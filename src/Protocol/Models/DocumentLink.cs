using System;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A document link is a range in a text document that links to an internal or external resource, like another
    /// text document or a web site.
    /// </summary>

    public class DocumentLink : ICanBeResolved, IRequest<DocumentLink>
    {
        /// <summary>
        /// The range this link applies to.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The uri this link points to. If missing a resolve request is sent later.
        /// </summary>
        [Optional]
        [JsonConverter(typeof(AbsoluteUriConverter))]
        public Uri Target { get; set; }

        /// </summary>
        /// A data entry field that is preserved on a document link between a
        /// DocumentLinkRequest and a DocumentLinkResolveRequest.
        /// </summary>
        [Optional]
        public JToken Data { get; set; }

        /// <summary>
        /// The tooltip text when you hover over this link.
        ///
        /// If a tooltip is provided, is will be displayed in a string that includes instructions on how to
        /// trigger the link, such as `{0} (ctrl + click)`. The specific instructions vary depending on OS,
        /// user settings, and localization.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public string Tooltip { get; set; }
    }
}
