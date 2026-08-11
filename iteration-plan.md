# System.Text.Json Migration Iteration Plan

The migration is split into independently testable and committable iterations. Each iteration preserves JSON-RPC, LSP, and DAP wire compatibility while removing a coherent Newtonsoft.Json dependency boundary.

## Working Rules

- Commit every completed iteration separately.
- Run focused tests immediately after the first implementation change in an iteration.
- Run the affected project suites and a full multi-target solution build before committing.
- Keep generated coverage manifests out of commits when test runs only reorder their keys.
- Do not remove transitional bridges until all callers have moved to System.Text.Json.

## Iterations

- [x] **1. Migrate LSP configuration payloads**
  - Change configuration responses and change notifications from `JToken` to `JsonElement`.
  - Migrate the server configuration pipeline and testing provider to System.Text.Json DOM APIs.
  - Cover JSON nulls, scalar settings, nested objects, arrays, and scoped configuration.

- [x] **2. Migrate LSP command arguments**
  - Replace `JArray` on `Command`, `ExecuteCommandParams`, and `IExecuteCommandParams`.
  - Preserve typed command argument serialization and handler deserialization.
  - Update command, code action, code lens, and completion integration coverage.

- [ ] **3. Migrate notebook metadata**
  - Replace the remaining `JObject` notebook metadata properties.
  - Verify arbitrary metadata objects and null or omitted metadata retain their wire shapes.

- [ ] **4. Migrate initialization and capability composition**
  - Replace raw initialize capabilities and mutable capability assembly with System.Text.Json DOM types.
  - Migrate client registration options and server capability merging without string-based JSON manipulation.
  - Preserve static, dynamic, experimental, and proposed capability behavior.

- [ ] **5. Migrate `LSPAny`, `LSPObject`, and `LSPArray`**
  - Replace the public Newtonsoft-backed arbitrary-value types with System.Text.Json-backed equivalents.
  - Preserve construction, conversion, equality, and serialization behavior where practical.
  - Document unavoidable source-breaking API changes.

- [ ] **6. Port the LSP serializer and converters**
  - Reimplement `LspSerializer` and `ProposedLspSerializer` on System.Text.Json.
  - Port converters in focused groups: scalars and enums, simple unions, discriminated objects, then complex edits and diagnostics.
  - Replace contract-resolver behavior for optional values, extension data, capability filtering, and tuples.
  - Verify existing fixtures remain wire-compatible.

- [ ] **7. Remove LSP transitional bridges**
  - Remove the Newtonsoft `JsonElement` converter and extension-data resolver bridge once unused.
  - Remove obsolete Newtonsoft serializer compatibility paths from LSP and JsonRpc.
  - Confirm no LSP project source references Newtonsoft.Json.

- [ ] **8. Complete DAP and repository cleanup**
  - Migrate remaining DAP public `JToken`, `JObject`, and `JArray` model surfaces and compatibility converters.
  - Remove the final Newtonsoft.Json package reference when all source consumers are gone.
  - Run all tests, packing, and the full multi-target solution build.

## Completion Criteria

- No production source file references Newtonsoft.Json or its DOM types.
- Newtonsoft.Json is absent from direct package references.
- Existing JSON fixtures and protocol integration tests retain their expected wire format.
- The full solution builds and all test suites pass for supported target frameworks.