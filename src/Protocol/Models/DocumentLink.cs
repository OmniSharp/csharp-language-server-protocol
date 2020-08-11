using System.Diagnostics;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A document link is a range in a text document that links to an internal or external resource, like another
    /// text document or a web site.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [Method(TextDocumentNames.DocumentLinkResolve, Direction.ClientToServer)]
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
        public DocumentUri Target { get; set; }

        /// <summary>
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

        private string DebuggerDisplay => $"{Range}{( Target != null ? $" {Target}" : "" )}{( string.IsNullOrWhiteSpace(Tooltip) ? $" {Tooltip}" : "" )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }

    /// <summary>
    /// A document link is a range in a text document that links to an internal or external resource, like another
    /// text document or a web site.
    /// </summary>
    public class DocumentLink<T> : ICanBeResolved
        where T : HandlerIdentity, new()
    {
        /// <summary>
        /// The range this link applies to.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The uri this link points to. If missing a resolve request is sent later.
        /// </summary>
        [Optional]
        public DocumentUri Target { get; set; }

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

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public T Data
        {
            get => ( (ICanBeResolved) this ).Data?.ToObject<T>();
            set => ( (ICanBeResolved) this ).Data = JToken.FromObject(value ?? new object());
        }

        JToken ICanBeResolved.Data { get; set; }

        public static implicit operator DocumentLink(DocumentLink<T> value) => new DocumentLink {
            Data = ( (ICanBeResolved) value ).Data,
            Range = value.Range,
            Target = value.Target,
            Tooltip = value.Tooltip,
        };

        public static implicit operator DocumentLink<T>(DocumentLink value)
        {
            var item = new DocumentLink<T> {
                Range = value.Range,
                Target = value.Target,
                Tooltip = value.Tooltip,
            };
            ( (ICanBeResolved) item ).Data = value.Data;
            return item;
        }
    }
}
