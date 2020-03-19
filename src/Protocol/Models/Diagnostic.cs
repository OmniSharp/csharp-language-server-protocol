using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class Diagnostic
    {
        /// <summary>
        /// The range at which the message applies.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// The diagnostic's severity. Can be omitted. If omitted it is up to the
        /// client to interpret diagnostics as error, warning, info or hint.
        /// </summary>
        [Optional]
        public DiagnosticSeverity? Severity { get; set; }

        /// <summary>
        /// The diagnostic's code. Can be omitted.
        /// </summary>
        [Optional]
        public DiagnosticCode Code { get; set; }

        /// <summary>
        /// A human-readable string describing the source of this
        /// diagnostic, e.g. 'typescript' or 'super lint'.
        /// </summary>
        [Optional]
        public string Source { get; set; }

        /// <summary>
        /// The diagnostic's message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Additional metadata about the diagnostic.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public Container<DiagnosticTag> Tags { get; set; }

        /// <summary>
        /// An array of related diagnostic information, e.g. when symbol-names within
        /// a scope collide all definitions can be marked via this property.
        /// </summary>
        [Optional]
        public Container<DiagnosticRelatedInformation> RelatedInformation { get; set; }
    }

    /**


##### <a href="#initiatingWorkDoneProgress" name="initiatingWorkDoneProgress" class="anchor">  Initiating Work Done Progress </a>

Work Done progress can be initiated in two different ways:

1. by the sender of a request (mostly clients) using the predefined `workDoneToken` property in the requests parameter literal.
1. by a server using the request `window/workDoneProgress/create`.

Consider a client sending a `textDocument/reference` request to a server and the client accepts work done progress reporting on that request. To signal this to the server the client would add a `workDoneToken` property to the reference request parameters. Something like this:

```json
{
	"textDocument": {
		"uri": "file:///folder/file.ts"
	},
	"position": {
		"line": 9,
		"character": 5
	},
	"context": {
		"includeDeclaration": true
	},
	// The token used to report work done progress.
	"workDoneToken": "1d546990-40a3-4b77-b134-46622995f6ae"
}
```

A server uses the `workDoneToken` to report progress for the specific `textDocument/reference`. For the above request the `$/progress` notification params look like this:

```json
{
	"token": "1d546990-40a3-4b77-b134-46622995f6ae",
	"value": {
		"title": "Finding references for A#foo",
		"cancellable": false,
		"message": "Processing file X.ts",
		"percentage": 0
	}
}
```

Server initiated work done progress works the same. The only difference is that the server requests a progress user interface using the `window/workDoneProgress/create` request providing a token that is afterwards used to report progress.

##### <a href="#signalingWorkDoneProgressReporting" name="signalingWorkDoneProgressReporting" class="anchor"> Signaling Work Done Progress Reporting </a>

To keep the protocol backwards compatible servers are only allowed to use work done progress reporting if the client signals corresponding support using the client capability `window.workDoneProgress` which is defined as follows:

```typescript
/// <summary>
	/// Window specific client capabilities.
/// </summary>
	window?: {
	/// <summary>
		/// Whether client supports handling progress notifications.
	/// </summary>
	boolean workDoneProgress? {get;set;}
	}
```

To avoid that clients set up a progress monitor user interface before sending a request but the server doesn't actually report any progress a server needs to signal work done progress reporting in the corresponding server capability. For the above find references example a server would signal such a support by setting the `referencesProvider` property in the server capabilities as follows:

```json
{
	"referencesProvider": {
		"workDoneProgress": true
	}
}
```

#### <a href="#workDoneProgressParams" name="workDoneProgressParams" class="anchor"> WorkDoneProgressParams </a>

A parameter literal used to pass a work done progress token.

```typescript
```

#### <a href="#workDoneProgressOptions" name="workDoneProgressOptions" class="anchor"> WorkDoneProgressOptions </a>

Options to signal work done progress support in server capabilities.

```typescript
public class WorkDoneProgressOptions {
boolean workDoneProgress? {get;set;}
}
```

#### <a href="#partialResults" name="partialResults" class="anchor"> Partial Result Progress </a>

> *Since version 3.15.0*

Partial results are also reported using the generic [`$/progress`](#progress) notification. The value payload of a partial result progress notification is in most cases the same as the final result. For example the `workspace/symbol` request as `SymbolInfomtion[]` as the result type. Partial result is therefore also of type `SymbolInformation[]`. Whether a client accept partial result notifications for a request is signaled by adding a `partialResultToken` to the request parameters. For example a `textDocument/reference` request support both work done and partial result progress might look like this:

```json
{
	"textDocument": {
		"uri": "file:///folder/file.ts"
	},
	"position": {
		"line": 9,
		"character": 5
	},
	"context": {
		"includeDeclaration": true
	},
	// The token used to report work done progress.
	"workDoneToken": "1d546990-40a3-4b77-b134-46622995f6ae",
	// The token used to report partial result progress.
	"partialResultToken": "5f6f349e-4f81-4a3b-afff-ee04bff96804"
}
```

The `partialResultToken` is then used to report partial results for the find references request.

If a server reports partial result via a corresponding `$/progress` the whole result must be reported using n `$/progress` notifications. The final response has to be empty in terms of result values. This avoids confusion about how the final result should be interpreted, e.g. as another partial result or as a replacing result.

#### <a href="#partialResultParams" name="partialResultParams" class="anchor"> PartialResultParams </a>

A parameter literal used to pass a partial result token.

```typescript
public class PartialResultParams {
/// <summary>
	/// An optional token that a server can use to report partial results (e.g. streaming) to
	/// the client.
/// </summary>
ProgressToken partialResultToken? {get;set;}
}
```


     */

}
