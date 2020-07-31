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
    class TextDocumentMatcher : IHandlerMatcher
    {
        private readonly ILogger<TextDocumentMatcher> _logger;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;

        public TextDocumentMatcher(ILogger<TextDocumentMatcher> logger, TextDocumentIdentifiers textDocumentIdentifiers)
        {
            _logger = logger;
            _textDocumentIdentifiers = textDocumentIdentifiers; ;
        }

        public IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors)
        {
            switch (parameters)
            {
                case ITextDocumentIdentifierParams textDocumentIdentifierParams:
                    {
                        if (textDocumentIdentifierParams.TextDocument?.Uri == null) break;
                        var attributes = GetTextDocumentAttributes(textDocumentIdentifierParams.TextDocument.Uri);

                        _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.LanguageId}:{x.Scheme}:{x.Uri}"));

                        return GetHandler(descriptors, attributes);
                    }
                case DidOpenTextDocumentParams openTextDocumentParams:
                    {
                        if (openTextDocumentParams.TextDocument?.Uri == null) break;
                        var attributes = new TextDocumentAttributes(openTextDocumentParams.TextDocument.Uri, openTextDocumentParams.TextDocument.LanguageId);

                        _logger.LogTrace("Created attribute {Attribute}", $"{attributes.LanguageId}:{attributes.Scheme}:{attributes.Uri}");

                        return GetHandler(descriptors, attributes);
                    }
                case DidChangeTextDocumentParams didChangeDocumentParams:
                    {
                        if (didChangeDocumentParams.TextDocument?.Uri == null) break;
                        // TODO: Do something with document version here?
                        var attributes = GetTextDocumentAttributes(didChangeDocumentParams.TextDocument.Uri);

                        _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.LanguageId}:{x.Scheme}:{x.Uri}"));

                        return GetHandler(descriptors, attributes);
                    }
            }

            return Enumerable.Empty<ILspHandlerDescriptor>();
        }

        private List<TextDocumentAttributes> GetTextDocumentAttributes(DocumentUri uri)
        {
            return _textDocumentIdentifiers
                .Select(x => x.GetTextDocumentAttributes(uri))
                .Where(x => x != null)
                .Distinct()
                .ToList();
        }

        private IEnumerable<ILspHandlerDescriptor> GetHandler(IEnumerable<ILspHandlerDescriptor> descriptors, IEnumerable<TextDocumentAttributes> attributes)
        {
            return attributes
                .SelectMany(x => GetHandler(descriptors, x));
        }

        private IEnumerable<ILspHandlerDescriptor> GetHandler(IEnumerable<ILspHandlerDescriptor> descriptors, TextDocumentAttributes attributes)
        {
            var method = descriptors.FirstOrDefault()?.Method;
            _logger.LogTrace("Looking for handler for descriptors {Method}", method);
            foreach (var descriptor in descriptors)
            {
                _logger.LogTrace("Checking handler {Method}:{Handler}", method, descriptor.ImplementationType.FullName);
                var registrationOptions = descriptor.RegistrationOptions as ITextDocumentRegistrationOptions;

                _logger.LogTrace("Registration options {OptionsName}", registrationOptions?.GetType().FullName);
                _logger.LogTrace("Document Selector {DocumentSelector}", registrationOptions?.DocumentSelector.ToString());
                if (registrationOptions?.DocumentSelector == null || registrationOptions.DocumentSelector.IsMatch(attributes))
                {
                    _logger.LogTrace("Handler Selected: {Handler} via {DocumentSelector} (targeting {HandlerInterface})", descriptor.ImplementationType.FullName, registrationOptions?.DocumentSelector?.ToString(), descriptor.HandlerType.FullName);
                    yield return descriptor;
                }
            }
        }
    }
}
