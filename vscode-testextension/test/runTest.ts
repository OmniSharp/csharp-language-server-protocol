import * as path from 'path';
import * as tmp from 'tmp-promise';

import { runTests } from '@vscode/test-electron';

async function main() {
	try {
		// The folder containing the Extension Manifest package.json
		// Passed to `--extensionDevelopmentPath`
		const extensionDevelopmentPath = path.resolve(__dirname, '../../');

		// The path to the extension test script
		// Passed to --extensionTestsPath
		const extensionTestsPath = path.resolve(__dirname, './suite/index');

		// The path to  the user data directory used by vscode
		// As the default one is too long (> 103 characters) on some CI setups, forcing it to a temporary and hopefully shorter one.
		const userDataDir = await tmp.dir({ unsafeCleanup: true });

		try {
			// Download VS Code, unzip it and run the integration test
			await runTests({
				extensionDevelopmentPath,
				extensionTestsPath,
				launchArgs: ["--user-data-dir", userDataDir.path]
			});
		}
		finally
		{
			await userDataDir.cleanup();
		}
	} catch (err) {
		console.error('Failed to run tests');
		console.error(err);
		process.exit(1);
	}
}

main();