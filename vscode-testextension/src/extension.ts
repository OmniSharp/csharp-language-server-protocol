/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
// tslint:disable
"use strict";

import * as path from "path";

import { workspace, Disposable, ExtensionContext } from "vscode";
import {
    LanguageClient,
    LanguageClientOptions,
    SettingMonitor,
    ServerOptions,
    TransportKind,
    InitializeParams
} from "vscode-languageclient";
import { Trace } from "vscode-jsonrpc";

export function activate(context: ExtensionContext) {
    // The server is implemented in node
    // let serverExe = 'dotnet';

    // let serverExe = 'D:\\Development\\Omnisharp\\csharp-language-server-protocol\\sample\\SampleServer\\bin\\Debug\\netcoreapp2.0\\win7-x64\\SampleServer.exe';
    let serverExe =
        "D:/Development/Omnisharp/omnisharp-roslyn/artifacts/publish/OmniSharp.Stdio.Driver/win7-x64/OmniSharp.exe";
    // The debug options for the server
    // let debugOptions = { execArgv: ['-lsp', '-d' };5

    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    let serverOptions: ServerOptions = {
        // run: { command: serverExe, args: ['-lsp', '-d'] },
        run: { command: serverExe, args: ["-lsp"] },
        // debug: { command: serverExe, args: ['-lsp', '-d'] }
        debug: { command: serverExe, args: ["-lsp"] }
    };

    // Options to control the language client
    let clientOptions: LanguageClientOptions = {
        // Register the server for plain text documents
        documentSelector: [
            {
                pattern: "**/*.cs"
            },
            {
                pattern: "**/*.csx"
            },
            {
                pattern: "**/*.cake"
            }
        ],
        synchronize: {
            // Synchronize the setting section 'languageServerExample' to the server
            configurationSection: "languageServerExample",
            fileEvents: workspace.createFileSystemWatcher("**/*.cs")
        }
    };

    // Create the language client and start the client.
    const client = new LanguageClient(
        "languageServerExample",
        "Language Server Example",
        serverOptions,
        clientOptions
    );
    // client.trace = Trace.Verbose;
    client.clientOptions.errorHandler;
    let disposable = client.start();

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}
