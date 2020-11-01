using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public partial class DocumentUri
    {
        private static readonly Regex WindowsPath =
            new Regex(@"^\w(?:\:|%3a)[\\|\/]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex SchemePattern = new Regex(@"^\w[[a-zA-Z\d+.-]*$");
        private static readonly Regex SingleSlashStart = new Regex(@"^\/");
        private static readonly Regex DoubleSlashStart = new Regex(@"^\/\/");

        private static bool IsHighSurrogate(int charCode) => 0xD800 <= charCode && charCode <= 0xDBFF;

        private static bool IsLowSurrogate(int charCode) => 0xDC00 <= charCode && charCode <= 0xDFFF;

        private static bool IsLowerAsciiHex(int code) => code >= CharCode.a && code <= CharCode.f;

        private static bool IsLowerAsciiLetter(int code) => code >= CharCode.a && code <= CharCode.z;

        private static bool IsUpperAsciiLetter(int code) => code >= CharCode.A && code <= CharCode.Z;

        private static bool IsAsciiLetter(int code) => IsLowerAsciiLetter(code) || IsUpperAsciiLetter(code);

        private static void _validateUri(DocumentUri ret, bool? strict)
        {
            // scheme, must be set
            if (string.IsNullOrWhiteSpace(ret.Scheme) && strict == true)
            {
                throw new UriFormatException(
                    $@"Scheme is missing: {{scheme: "", authority: ""{ret.Authority}"", path: ""{ret.Path}"", query: ""${ret.Query}"", fragment: ""{ret.Fragment}""}}"
                );
            }

            // scheme, https://tools.ietf.org/html/rfc3986#section-3.1
            // ALPHA *( ALPHA / DIGIT / "+" / "-" / "." )
            if (!string.IsNullOrWhiteSpace(ret.Scheme) && !SchemePattern.IsMatch(ret.Scheme!))
            {
                throw new UriFormatException("Scheme contains illegal characters.");
            }

            // path, http://tools.ietf.org/html/rfc3986#section-3.3
            // If a URI contains an authority component, then the path component
            // must either be empty or begin with a slash ("/") character.  If a URI
            // does not contain an authority component, then the path cannot begin
            // with two slash characters ("//").
            if (!string.IsNullOrWhiteSpace(ret.Path))
            {
                if (!string.IsNullOrWhiteSpace(ret.Authority))
                {
                    if (!SingleSlashStart.IsMatch(ret.Path))
                    {
                        throw new UriFormatException(
                            "If a URI contains an authority component, then the path component must either be empty or begin with a slash (\"/\") character"
                        );
                    }
                }
                else
                {
                    if (DoubleSlashStart.IsMatch(ret.Path))
                    {
                        throw new UriFormatException(
                            "If a URI does not contain an authority component, then the path cannot begin with two slash characters (\"//\")"
                        );
                    }
                }
            }
        }

        private static string? SchemeFix(string? scheme, bool? strict)
        {
            if (string.IsNullOrWhiteSpace(scheme) && strict != true)
            {
                return Uri.UriSchemeFile;
            }

            return scheme;
        }

        private const string Empty = "";
        private const char Slash = '/';
        private const string StrSlash = "/";

        private static readonly Regex Regexp =
            new Regex(@"^(([^:/?#]+?):)?(\/\/([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?");

        private static string ReferenceResolution(string? scheme, string path)
        {
            // the slash-character is our "default base' as we don"t
            // support constructing URIs relative to other URIs. This
            // also means that we alter and potentially break paths.
            // see https://tools.ietf.org/html/rfc3986#section-5.1.4
            if (scheme == Uri.UriSchemeHttps || scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeFile)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return StrSlash;
                }

                if (path[0] != Slash)
                {
                    return Slash + path;
                }
            }

            return path;
        }

        private static readonly IDictionary<int, string> EncodeTable = new Dictionary<int, string> {
            [CharCode.Colon] = "%3A", // gen-delims
            [CharCode.Slash] = "%2F",
            [CharCode.QuestionMark] = "%3F",
            [CharCode.Hash] = "%23",
            [CharCode.OpenSquareBracket] = "%5B",
            [CharCode.CloseSquareBracket] = "%5D",
            [CharCode.AtSign] = "%40",

            [CharCode.ExclamationMark] = "%21", // sub-delims
            [CharCode.DollarSign] = "%24",
            [CharCode.Ampersand] = "%26",
            [CharCode.SingleQuote] = "%27",
            [CharCode.OpenParen] = "%28",
            [CharCode.CloseParen] = "%29",
            [CharCode.Asterisk] = "%2A",
            [CharCode.Plus] = "%2B",
            [CharCode.Comma] = "%2C",
            [CharCode.Semicolon] = "%3B",
            [CharCode.Equals] = "%3D",

            [CharCode.Space] = "%20",
        };

        private static string EncodeUriComponentFast(string uriComponent, bool allowSlash)
        {
            StringBuilder? res = null;
            var nativeEncodePos = -1;

            for (var pos = 0; pos < uriComponent.Length; pos++)
            {
                var code = uriComponent[pos];

                // unreserved characters: https://tools.ietf.org/html/rfc3986#section-2.3
                if (
                    code >= CharCode.a && code <= CharCode.z
                 || code >= CharCode.A && code <= CharCode.Z
                 || code >= CharCode.Digit0 && code <= CharCode.Digit9
                 || code == CharCode.Dash
                 || code == CharCode.Period
                 || code == CharCode.Underline
                 || code == CharCode.Tilde
                 || allowSlash && code == CharCode.Slash
                 || allowSlash && ( pos == 1 || pos == 2 ) && (
                        uriComponent.Length >= 3 && uriComponent[0] == CharCode.Slash &&
                        uriComponent[2] == CharCode.Colon
                     || uriComponent.Length >= 2 && uriComponent[1] == CharCode.Colon
                    )
                )
                {
                    // check if we are delaying native encode
                    if (nativeEncodePos != -1)
                    {
                        res ??= new StringBuilder();
                        res.Append(Uri.EscapeDataString(uriComponent.Substring(nativeEncodePos, pos - nativeEncodePos)));
                        nativeEncodePos = -1;
                    }

                    // check if we write into a new string (by default we try to return the param)
                    res?.Append(uriComponent[pos]);
                }
                else
                {
                    // encoding needed, we need to allocate a new string
                    if (res == null)
                    {
                        res ??= new StringBuilder();
                        res.Append(uriComponent.Substring(0, pos));
                    }

                    // check with default table first
                    if (EncodeTable.TryGetValue(code, out var escaped))
                    {
                        // check if we are delaying native encode
                        if (nativeEncodePos != -1)
                        {
                            res.Append(Uri.EscapeDataString(uriComponent.Substring(nativeEncodePos, pos - nativeEncodePos)));
                            nativeEncodePos = -1;
                        }

                        // append escaped variant to result
                        res.Append(escaped);
                    }
                    else if (nativeEncodePos == -1)
                    {
                        // use native encode only when needed
                        nativeEncodePos = pos;
                    }
                }
            }

            if (nativeEncodePos != -1)
            {
                res ??= new StringBuilder();
                res.Append(Uri.EscapeDataString(uriComponent.Substring(nativeEncodePos)));
            }

            return res != null ? res.ToString() : uriComponent;
        }

        private static string EncodeUriComponentMinimal(string path)
        {
            StringBuilder? res = null;
            for (var pos = 0; pos < path.Length; pos++)
            {
                var code = path[pos];
                if (code == CharCode.Hash || code == CharCode.QuestionMark)
                {
                    if (res == null)
                    {
                        res = new StringBuilder(path.Substring(0, pos));
                    }

                    res.Append(EncodeTable[code]);
                }
                else
                {
                    res?.Append(path[pos]);
                }
            }

            return res != null ? res.ToString() : path;
        }

        /// <summary>
        /// Compute `fsPath` for the given uri
        /// </summary>
        private static string UriToFsPath(DocumentUri uri, bool keepDriveLetterCasing)
        {
            string value;
            if (!string.IsNullOrWhiteSpace(uri.Authority) && uri.Path.Length > 1 && uri.Scheme == "file")
            {
                // unc path: file://shares/c$/far/boo
                value = $@"\\{uri.Authority}{uri.Path}";
            }
            else if (
                uri.Path.Length >= 3
             && uri.Path[0] == CharCode.Slash
             && ( uri.Path[1] >= CharCode.A && uri.Path[1] <= CharCode.Z ||
                  uri.Path[1] >= CharCode.a && uri.Path[1] <= CharCode.z )
             && uri.Path[2] == CharCode.Colon
            )
            {
                if (!keepDriveLetterCasing)
                {
                    // windows drive letter: file:///c:/far/boo
                    value = char.ToLowerInvariant(uri.Path[1]) + uri.Path.Substring(2);
                }
                else
                {
                    value = uri.Path.Substring(1);
                }
            }
            else
            {
                // other path
                value = uri.Path;
            }

            if (WindowsPath.IsMatch(value) || value.StartsWith("\\\\"))
            {
                return value.Replace("/", "\\");
            }

            return value;
        }

        /// <summary>
        /// Create the external version of a uri
        /// </summary>
        private static string _asFormatted(DocumentUri uri, bool skipEncoding)
        {
            string Encoder(string p, bool allowSlash)
            {
                return !skipEncoding ? EncodeUriComponentFast(p, allowSlash) : EncodeUriComponentMinimal(p);
            }

            var res = new StringBuilder();
            var (scheme, authority, path, query, fragment) = uri;
            if (!string.IsNullOrWhiteSpace(scheme))
            {
                res.Append(scheme);
                res.Append(":");
            }

            if (!string.IsNullOrWhiteSpace(authority) || scheme == "file")
            {
                res.Append(Slash);
                res.Append(Slash);
            }

            if (!string.IsNullOrWhiteSpace(authority))
            {
                var idx = authority.IndexOf("@", StringComparison.Ordinal);
                if (idx != -1)
                {
                    // <user>@<auth>
                    var userinfo = authority.Substring(0, idx);
                    authority = authority.Substring(idx + 1);
                    idx = userinfo.IndexOf(":", StringComparison.Ordinal);
                    if (idx == -1)
                    {
                        res.Append(Encoder(userinfo, false));
                    }
                    else
                    {
                        // <user>:<pass>@<auth>
                        res.Append(Encoder(userinfo.Substring(0, idx), false));
                        res.Append(":");
                        res.Append(Encoder(userinfo.Substring(idx + 1), false));
                    }

                    res.Append("@");
                }

                authority = authority.ToLowerInvariant();
                idx = authority.IndexOf(":", StringComparison.Ordinal);
                if (idx == -1)
                {
                    res.Append(Encoder(authority, false));
                }
                else
                {
                    // <auth>:<port>
                    res.Append(Encoder(authority.Substring(0, idx), false));
                    res.Append(authority.Substring(idx));
                }
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                var appended = false;
                // lower-case windows drive letters in /C:/fff or C:/fff
                if (path.Length >= 3 && path[0] == CharCode.Slash && path[2] == CharCode.Colon)
                {
                    var code = path[1];
                    if (code >= CharCode.A && code <= CharCode.Z)
                    {
                        appended = true;
                        res.Append("/");
                        res.Append(Convert.ToChar(code + 32));
                        res.Append(":");
                        res.Append(Encoder(path.Substring(3), true));
//                         path = $"/{Convert.ToChar(code + 32)}:{path.Substring(3)}"; // "/c:".length == 3
                    }
                }
                else if (path.Length >= 2 && path[1] == CharCode.Colon)
                {
                    var code = path[0];
                    if (code >= CharCode.A && code <= CharCode.Z)
                    {
                        appended = true;
                        res.Append(Convert.ToChar(code + 32));
                        res.Append(":");
                        res.Append(Encoder(path.Substring(2), true));
//                         path = $"{Convert.ToChar(code + 32)}:{path.Substring(2)}"; // "/c:".length == 3
                    }
                }

                if (!appended)
                {
                    res.Append(Encoder(path, true));
                }
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                res.Append("?");
                res.Append(Encoder(query, false));
            }

            if (!string.IsNullOrWhiteSpace(fragment))
            {
                res.Append("#");
                res.Append(!skipEncoding ? EncodeUriComponentFast(fragment, false) : fragment);
            }

            return res.ToString();
        }

        private static string DecodeUriComponentGraceful(string str)
        {
            try
            {
                return Uri.UnescapeDataString(str);
            }
            catch
            {
                if (str.Length > 3)
                {
                    return str.Substring(0, 3) + DecodeUriComponentGraceful(str.Substring(3));
                }

                return str;
            }
        }

        private static readonly Regex REncodedAsHex =
            new Regex(@"(%[0-9A-Za-z][0-9A-Za-z])+", RegexOptions.Multiline | RegexOptions.Compiled);

        private static string PercentDecode(string str)
        {
            if (!REncodedAsHex.IsMatch(str))
            {
                return str;
            }

            return REncodedAsHex.Replace(str, match => DecodeUriComponentGraceful(match.Value));
        }

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }
}
