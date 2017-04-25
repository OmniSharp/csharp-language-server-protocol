const cp = require('child_process')
const {join} = require('path');
const {AutoLanguageClient} = require('atom-languageclient')

class OmnisharpLanguageServer extends AutoLanguageClient {
  getGrammarScopes () { return [ 'source.xml' ] }
  getLanguageName () { return 'XML' }
  getServerName () { return 'OmniSharp' }

  startServerProcess () {
    return cp.spawn('node', [ join(__dirname, "../../sample/SampleServer/bin/Debug/netcoreapp1.1/win7-x64/SampleServer.exe") ])
  }
}

module.exports = new OmnisharpLanguageServer()
