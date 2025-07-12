using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    internal class NotebookDocumentMatcher : IHandlerMatcher
    {
        private readonly ILogger<NotebookDocumentMatcher> _logger;
        private readonly NotebookDocumentIdentifiers _notebookDocumentIdentifiers;

        public NotebookDocumentMatcher(ILogger<NotebookDocumentMatcher> logger, NotebookDocumentIdentifiers notebookDocumentIdentifiers)
        {
            _logger = logger;
            _notebookDocumentIdentifiers = notebookDocumentIdentifiers;
        }

        public IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors)
        {
            switch (parameters)
            {
                case INotebookDocumentIdentifierParams notebookDocumentIdentifierParams:
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    if (notebookDocumentIdentifierParams.NotebookDocument?.Uri is null) break;
                    var attributes = GetNotebookDocumentAttributes(notebookDocumentIdentifierParams.NotebookDocument.Uri);

                    _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.NotebookType}:{x.Scheme}:{x.Uri}"));

                    return GetHandler(descriptors, attributes);
                }
                case DidOpenNotebookDocumentParams openNotebookDocumentParams:
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    if (openNotebookDocumentParams.NotebookDocument?.Uri is null) break;
                    var attributes = new List<NotebookDocumentAttributes>()
                    {
                        new (
                            openNotebookDocumentParams.NotebookDocument.Uri, openNotebookDocumentParams.NotebookDocument.NotebookType
                        )
                    };
                    attributes.AddRange(
                        openNotebookDocumentParams.CellTextDocuments
                                                  .Select(z => new NotebookDocumentAttributes(z.LanguageId, z.Uri))
                                                  .Distinct()
                    );

                    _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.NotebookType}:{x.Scheme}:{x.Uri}"));

                    return GetHandler(descriptors, attributes);
                }
                case DidChangeNotebookDocumentParams didChangeDocumentParams:
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    if (didChangeDocumentParams.NotebookDocument?.Uri is null) break;
                    // TODO: Do something with document version here?
                    var attributes = GetNotebookDocumentAttributes(didChangeDocumentParams.NotebookDocument.Uri);
                    
                    attributes.AddRange(
                        didChangeDocumentParams.Change.Cells.Structure?.Array.Cells?
                                               .Select(z => new NotebookDocumentAttributes(z.Document))
                                               .Distinct() ?? Array.Empty<NotebookDocumentAttributes>()
                    );
                    attributes.AddRange(
                        didChangeDocumentParams.Change.Cells.Data?
                                               .Select(z => new NotebookDocumentAttributes(z.Document))
                                               .Distinct() ?? Array.Empty<NotebookDocumentAttributes>()
                    );

                    _logger.LogTrace("Found attributes {Count}, {Attributes}", attributes.Count, attributes.Select(x => $"{x.NotebookType}:{x.Scheme}:{x.Uri}"));

                    return GetHandler(descriptors, attributes);
                }
            }

            return Enumerable.Empty<ILspHandlerDescriptor>();
        }

        private List<NotebookDocumentAttributes> GetNotebookDocumentAttributes(DocumentUri uri) =>
            _notebookDocumentIdentifiers
               .Select(x => x.GetNotebookDocumentAttributes(uri))
               .Where(x => x is not null)
               .ToList();

        private IEnumerable<ILspHandlerDescriptor> GetHandler(IEnumerable<ILspHandlerDescriptor> descriptors, IEnumerable<NotebookDocumentAttributes> attributes) =>
            attributes.SelectMany(z => GetHandler(descriptors, z)).Distinct();

        private IEnumerable<ILspHandlerDescriptor> GetHandler(IEnumerable<ILspHandlerDescriptor> descriptors, NotebookDocumentAttributes attributes)
        {
            var lspHandlerDescriptors = descriptors as ILspHandlerDescriptor[] ?? descriptors.ToArray();
            var method = lspHandlerDescriptors.FirstOrDefault()?.Method;
            _logger.LogTrace("Looking for handler for descriptors {Method}", method);
            foreach (var descriptor in lspHandlerDescriptors)
            {
                _logger.LogTrace("Checking handler {Method}:{Handler}", method, descriptor.ImplementationType.FullName);
                var registrationOptions = descriptor.RegistrationOptions as INotebookDocumentRegistrationOptions;

                _logger.LogTrace("Registration options {OptionsName}", registrationOptions?.GetType().FullName);
                var selector = registrationOptions?.NotebookSelector?.FirstOrDefault(s => s.IsMatch(attributes));
                _logger.LogTrace("Document Selector {NotebookDocumentSelector}", selector?.ToString());
                if (registrationOptions?.NotebookSelector is null || selector is not null)
                {
                    _logger.LogTrace(
                        "Handler Selected: {Handler} {Id} via {NotebookDocumentSelector} (targeting {HandlerInterface})",
                        descriptor.ImplementationType.FullName,
                        descriptor.Handler is ICanBeIdentifiedHandler h ? h.Id.ToString() : string.Empty,
                        selector?.ToString(),
                        descriptor.HandlerType.FullName
                    );
                    yield return descriptor;
                }
            }
        }
    }
}
