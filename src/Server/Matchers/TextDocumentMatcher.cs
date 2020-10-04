using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    internal class TextDocumentMatcher : IHandlerMatcher
    {
        private readonly ILogger<TextDocumentMatcher> _logger;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;

        public TextDocumentMatcher(ILogger<TextDocumentMatcher> logger, TextDocumentIdentifiers textDocumentIdentifiers)
        {
            _logger = logger;
            _textDocumentIdentifiers = textDocumentIdentifiers;
        }

        public IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors)
        {
            switch (parameters)
            {
                case ITextDocumentIdentifierParams textDocumentIdentifierParams:
                {
                    if (textDocumentIdentifierParams.TextDocument?.Uri is null) break;
                    var attributes = GetTextDocumentAttributes(textDocumentIdentifierParams.TextDocument.Uri);

                    _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.LanguageId}:{x.Scheme}:{x.Uri}"));

                    return GetHandler(descriptors, attributes);
                }
                case DidOpenTextDocumentParams openTextDocumentParams:
                {
                    if (openTextDocumentParams.TextDocument?.Uri is null) break;
                    var attributes = new TextDocumentAttributes(openTextDocumentParams.TextDocument.Uri, openTextDocumentParams.TextDocument.LanguageId);

                    _logger.LogTrace("Created attribute {Attribute}", $"{attributes.LanguageId}:{attributes.Scheme}:{attributes.Uri}");

                    return GetHandler(descriptors, attributes);
                }
                case DidChangeTextDocumentParams didChangeDocumentParams:
                {
                    if (didChangeDocumentParams.TextDocument?.Uri is null) break;
                    // TODO: Do something with document version here?
                    var attributes = GetTextDocumentAttributes(didChangeDocumentParams.TextDocument.Uri);

                    _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.LanguageId}:{x.Scheme}:{x.Uri}"));

                    return GetHandler(descriptors, attributes);
                }
            }

            return Enumerable.Empty<ILspHandlerDescriptor>();
        }

        private List<TextDocumentAttributes> GetTextDocumentAttributes(DocumentUri uri) =>
            _textDocumentIdentifiers
               .Select(x => x.GetTextDocumentAttributes(uri))
               .Where(x => x != null)
               .ToList();

        private IEnumerable<ILspHandlerDescriptor> GetHandler(IEnumerable<ILspHandlerDescriptor> descriptors, IEnumerable<TextDocumentAttributes> attributes) =>
            attributes.SelectMany(z => GetHandler(descriptors, z)).Distinct();

        private IEnumerable<ILspHandlerDescriptor> GetHandler(IEnumerable<ILspHandlerDescriptor> descriptors, TextDocumentAttributes attributes)
        {
            var lspHandlerDescriptors = descriptors as ILspHandlerDescriptor[] ?? descriptors.ToArray();
            var method = lspHandlerDescriptors.FirstOrDefault()?.Method;
            _logger.LogTrace("Looking for handler for descriptors {Method}", method);
            foreach (var descriptor in lspHandlerDescriptors)
            {
                _logger.LogTrace("Checking handler {Method}:{Handler}", method, descriptor.ImplementationType.FullName);
                var registrationOptions = descriptor.RegistrationOptions as ITextDocumentRegistrationOptions;

                _logger.LogTrace("Registration options {OptionsName}", registrationOptions?.GetType().FullName);
                _logger.LogTrace("Document Selector {DocumentSelector}", registrationOptions?.DocumentSelector?.ToString() ?? string.Empty);
                if (registrationOptions?.DocumentSelector is null || registrationOptions.DocumentSelector.IsMatch(attributes))
                {
                    _logger.LogTrace(
                        "Handler Selected: {Handler} {Id} via {DocumentSelector} (targeting {HandlerInterface})",
                        descriptor.ImplementationType.FullName,
                        descriptor.Handler is ICanBeIdentifiedHandler h ? h.Id.ToString() : string.Empty,
                        registrationOptions?.DocumentSelector?.ToString(),
                        descriptor.HandlerType.FullName
                    );
                    yield return descriptor;
                }
            }
        }
    }
}
