using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    public class TextDocumentMatcher : IHandlerMatcher
    {
        private readonly ILogger _logger;
        private readonly Func<IEnumerable<ITextDocumentSyncHandler>> _getSyncHandlers;

        public TextDocumentMatcher(ILogger logger, Func<IEnumerable<ITextDocumentSyncHandler>> getSyncHandlers)
        {
            _logger = logger;
            _getSyncHandlers = getSyncHandlers;
        }

        public IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors)
        {
            switch (parameters)
            {
                case ITextDocumentIdentifierParams textDocumentIdentifierParams:
                    {
                        var attributes = GetTextDocumentAttributes(descriptors, textDocumentIdentifierParams.TextDocument.Uri);

                        _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.LanguageId}:{x.Scheme}:{x.Uri}"));

                        return GetHandler(descriptors, attributes);
                    }
                case DidOpenTextDocumentParams openTextDocumentParams:
                    {
                        var attributes = new TextDocumentAttributes(openTextDocumentParams.TextDocument.Uri, openTextDocumentParams.TextDocument.LanguageId);

                        _logger.LogTrace("Created attribute {Attribute}", $"{attributes.LanguageId}:{attributes.Scheme}:{attributes.Uri}");

                        return GetHandler(descriptors, attributes);
                    }
                case DidChangeTextDocumentParams didChangeDocumentParams:
                    {
                        // TODO: Do something with document version here?
                        var attributes = GetTextDocumentAttributes(descriptors, didChangeDocumentParams.TextDocument.Uri);

                        _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.LanguageId}:{x.Scheme}:{x.Uri}"));

                        return GetHandler(descriptors, attributes);
                    }
            }

            return Enumerable.Empty<ILspHandlerDescriptor>();
        }

        private List<TextDocumentAttributes> GetTextDocumentAttributes(IEnumerable<ILspHandlerDescriptor> method, Uri uri)
        {
            return _getSyncHandlers()
                .Select(x => x.GetTextDocumentAttributes(uri))
                .Where(x => x != null)
                .Distinct()
                .ToList();
        }

        private IEnumerable<ILspHandlerDescriptor> GetHandler(IEnumerable<ILspHandlerDescriptor> method, IEnumerable<TextDocumentAttributes> attributes)
        {
            return attributes
                .SelectMany(x => GetHandler(method, x)); 
        }

        private IEnumerable<ILspHandlerDescriptor> GetHandler(IEnumerable<ILspHandlerDescriptor> method, TextDocumentAttributes attributes)
        {
            _logger.LogTrace("Looking for handler for method {Method}", method);
            foreach (var handler in method)
            {
                _logger.LogTrace("Checking handler {Method}:{Handler}", method, handler.Handler.GetType().FullName);
                var registrationOptions = handler.Registration.RegisterOptions as TextDocumentRegistrationOptions;

                _logger.LogTrace("Registration options {OptionsName}", registrationOptions?.GetType().FullName);
                _logger.LogTrace("Document Selector {DocumentSelector}", registrationOptions?.DocumentSelector.ToString());
                if (registrationOptions?.DocumentSelector == null || registrationOptions.DocumentSelector.IsMatch(attributes))
                {
                    _logger.LogTrace("Handler Selected: {Handler} via {DocumentSelector} (targeting {HandlerInterface})", handler.Handler.GetType().FullName, registrationOptions.DocumentSelector.ToString(), handler.HandlerType.FullName);
                    yield return handler;
                }
            }
        }
    }
}
