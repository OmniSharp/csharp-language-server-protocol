using System.ComponentModel;
using System.Diagnostics;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A document link is a range in a text document that links to an internal or external resource, like another
    /// text document or a web site.
    /// </summary>
    [Method(TextDocumentNames.DocumentLinkResolve, Direction.ClientToServer)]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
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
        public JObject Data { get; set; }

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

        private string DebuggerDisplay => $"{Range}{(Target != null ? $" {Target}" : "")}{(string.IsNullOrWhiteSpace(Tooltip) ? $" {Tooltip}" : "")}";
        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;

        /// <summary>
        /// Convert from a <see cref="CodeLens"/>
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal DocumentLink<T> From<T>(ISerializer serializer) where T : class
        {
            return new DocumentLink<T>() {
                Range = Range,
                Target = Target,
                Tooltip = Tooltip,
                Data = Data?.ToObject<T>(serializer.JsonSerializer)
            };
        }
    }
    /// <summary>
    /// A document link is a range in a text document that links to an internal or external resource, like another
    /// text document or a web site.
    /// </summary>
    public class DocumentLink<T> : DocumentLink where T : class
    {
        /// <summary>
        /// A data entry field that is preserved on a document link between a
        /// DocumentLinkRequest and a DocumentLinkResolveRequest.
        /// </summary>
        [Optional]
        public new T Data { get; set; }

        /// <summary>
        /// Convert to a <see cref="CodeLens"/>
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal DocumentLink To(ISerializer serializer)
        {
            if (Data != null)
            {
                base.Data = JObject.FromObject(Data, serializer.JsonSerializer);
            }

            return this;
        }
    }
}
