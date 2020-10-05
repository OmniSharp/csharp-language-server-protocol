using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Xunit;

namespace Lsp.Tests
{
    public class VsCodeDocumentUriTests
    {
        [Fact(DisplayName = "file#toString")]
        public void FileToString()
        {
        }

        [Fact(DisplayName = "file#toString")]
        public void file_toString()
        {
            DocumentUri.File("c:/win/path").ToString().Should().Be("file:///c:/win/path");
            DocumentUri.File("C:/win/path").ToString().Should().Be("file:///c:/win/path");
            DocumentUri.File("c:/win/path/").ToString().Should().Be("file:///c:/win/path/");
            DocumentUri.File("/c:/win/path").ToString().Should().Be("file:///c:/win/path");
        }

        [Fact(DisplayName = "DocumentUri.File (win-special)")]
        public void URI_file__win_special_()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                DocumentUri.File("c:\\win\\path").ToString().Should().Be("file:///c:/win/path");
                DocumentUri.File("c:\\win/path").ToString().Should().Be("file:///c:/win/path");
            }
            else
            {
                DocumentUri.File("/usr/win\\path").ToString().Should().Be("file:///usr/win%5Cpath");
                DocumentUri.File("/usr/win/path").ToString().Should().Be("file:///usr/win/path");
            }
        }

        [Fact(DisplayName = "file#fsPath (win-special)")]
        public void file_fsPath__win_special_()
        {
            DocumentUri.File("c:\\win\\path").GetFileSystemPath().Should().Be("c:\\win\\path");
            DocumentUri.File("c:\\win/path").GetFileSystemPath().Should().Be("c:\\win\\path");

            DocumentUri.File("c:/win/path").GetFileSystemPath().Should().Be("c:\\win\\path");
            DocumentUri.File("c:/win/path/").GetFileSystemPath().Should().Be("c:\\win\\path\\");
            DocumentUri.File("C:/win/path").GetFileSystemPath().Should().Be("c:\\win\\path");
            DocumentUri.File("/c:/win/path").GetFileSystemPath().Should().Be("c:\\win\\path");
            DocumentUri.File("./c/win/path").GetFileSystemPath().Should().Be("/./c/win/path");
        }

        [Fact(DisplayName = "URI#fsPath - no `fsPath` when no `path`")]
        public void URI_fsPath___no_fsPath_when_no_path()
        {
            var value = DocumentUri.Parse("file://%2Fhome%2Fticino%2Fdesktop%2Fcpluscplus%2Ftest.cpp");
            value.Authority.Should().Be("/home/ticino/desktop/cpluscplus/test.cpp");
            value.Path.Should().Be("/");
            value.GetFileSystemPath().Should().Be("/");
        }

        [Fact(DisplayName = "http#toString")]
        public void http_toString()
        {
            DocumentUri.From(
                new DocumentUriComponents
                    { Scheme = "http", Authority = "www.msft.com", Path = "/my/path" }
            ).ToString().Should().Be(
                "http://www.msft.com/my/path"
            );

            DocumentUri.From(
                new DocumentUriComponents
                    { Scheme = "http", Authority = "www.msft.com", Path = "/my/path" }
            ).ToString().Should().Be(
                "http://www.msft.com/my/path"
            );

            DocumentUri.From(
                new DocumentUriComponents
                    { Scheme = "http", Authority = "www.MSFT.com", Path = "/my/path" }
            ).ToString().Should().Be(
                "http://www.msft.com/my/path"
            );

            DocumentUri.From(new DocumentUriComponents { Scheme = "http", Authority = "", Path = "my/path" })
                       .ToString().Should().Be("http:/my/path");

            DocumentUri.From(new DocumentUriComponents { Scheme = "http", Authority = "", Path = "/my/path" })
                       .ToString().Should().Be("http:/my/path");
            //http://a-test-site.com/#test=true

            DocumentUri.From(
                            new DocumentUriComponents
                                { Scheme = "http", Authority = "a-test-site.com", Path = "/", Query = "test=true" }
                        ).ToString()
                       .Should()
                       .Be(
                            "http://a-test-site.com/?test%3Dtrue"
                        );

            DocumentUri.From(
                            new DocumentUriComponents {
                                Scheme = "http", Authority = "a-test-site.com", Path = "/", Query = "", Fragment = "test=true"
                            }
                        )
                       .ToString().Should().Be("http://a-test-site.com/#test%3Dtrue");
        }

        [Fact(DisplayName = "http#toString, encode=FALSE")]
        public void http_toString__encode_FALSE()
        {
            DocumentUri.From(
                            new DocumentUriComponents
                                { Scheme = "http", Authority = "a-test-site.com", Path = "/", Query = "test=true" }
                        )
                       .ToUnencodedString().Should().Be("http://a-test-site.com/?test=true");

            DocumentUri.From(
                            new DocumentUriComponents {
                                Scheme = "http", Authority = "a-test-site.com", Path = "/", Query = "", Fragment = "test=true"
                            }
                        )
                       .ToUnencodedString().Should().Be("http://a-test-site.com/#test=true");

            DocumentUri.From(
                new DocumentUriComponents
                    { Scheme = "http", Path = "/api/files/test.me", Query = "t=1234" }
            ).ToUnencodedString().Should().Be(
                "http:/api/files/test.me?t=1234"
            );

            var value = DocumentUri.Parse("file://shares/pröjects/c%23/#l12");
            value.Authority.Should().Be("shares");
            value.Path.Should().Be("/pröjects/c#/");
            value.Fragment.Should().Be("l12");
            value.ToString().Should().Be("file://shares/pr%C3%B6jects/c%23/#l12");
            value.ToUnencodedString().Should().Be("file://shares/pröjects/c%23/#l12");

            var uri2 = DocumentUri.Parse(value.ToUnencodedString());
            var uri3 = DocumentUri.Parse(value.ToString());
            uri2.Authority.Should().Be(uri3.Authority);
            uri2.Path.Should().Be(uri3.Path);
            uri2.Query.Should().Be(uri3.Query);
            uri2.Fragment.Should().Be(uri3.Fragment);
        }

        [Fact(DisplayName = "with, identity")]
        public void with__identity()
        {
            var uri = DocumentUri.Parse("foo:bar/path");

            var uri2 = uri.With(new DocumentUriComponents());
            Assert.True(uri == uri2);
            uri2 = uri.With(new DocumentUriComponents { Scheme = "foo", Path = "bar/path" });
            Assert.True(uri == uri2);
        }

        [Fact(DisplayName = "with, changes")]
        public void with__changes()
        {
            DocumentUri.Parse("before:some/file/path").With(new DocumentUriComponents { Scheme = "after" })
                       .ToString().Should().Be("after:some/file/path");

            DocumentUri.From(new DocumentUriComponents { Scheme = "s" }).With(
                new DocumentUriComponents
                    { Scheme = "http", Path = "/api/files/test.me", Query = "t=1234" }
            ).ToString().Should().Be(
                "http:/api/files/test.me?t%3D1234"
            );

            DocumentUri.From(new DocumentUriComponents { Scheme = "s" }).With(
                            new DocumentUriComponents
                                { Scheme = "http", Authority = "", Path = "/api/files/test.me", Query = "t=1234", Fragment = "" }
                        )
                       .ToString().Should().Be("http:/api/files/test.me?t%3D1234");

            DocumentUri.From(new DocumentUriComponents { Scheme = "s" }).With(
                            new DocumentUriComponents {
                                Scheme = "https", Authority = "", Path = "/api/files/test.me", Query = "t=1234", Fragment = ""
                            }
                        )
                       .ToString().Should().Be("https:/api/files/test.me?t%3D1234");

            DocumentUri.From(new DocumentUriComponents { Scheme = "s" }).With(
                            new DocumentUriComponents
                                { Scheme = "HTTP", Authority = "", Path = "/api/files/test.me", Query = "t=1234", Fragment = "" }
                        )
                       .ToString().Should().Be("HTTP:/api/files/test.me?t%3D1234");

            DocumentUri.From(new DocumentUriComponents { Scheme = "s" }).With(
                            new DocumentUriComponents {
                                Scheme = "HTTPS", Authority = "", Path = "/api/files/test.me", Query = "t=1234", Fragment = ""
                            }
                        )
                       .ToString().Should().Be("HTTPS:/api/files/test.me?t%3D1234");

            DocumentUri.From(new DocumentUriComponents { Scheme = "s" }).With(
                            new DocumentUriComponents
                                { Scheme = "boo", Authority = "", Path = "/api/files/test.me", Query = "t=1234", Fragment = "" }
                        )
                       .ToString().Should().Be("boo:/api/files/test.me?t%3D1234");
        }

        [Fact(DisplayName = "with, remove components #8465")]
        public void with__remove_components__8465()
        {
            DocumentUri.Parse("scheme://authority/path").With(new DocumentUriComponents { Authority = "" })
                       .ToString().Should().Be("scheme:/path");

            DocumentUri.Parse("scheme:/path").With(new DocumentUriComponents { Authority = "authority" })
                       .With(new DocumentUriComponents { Authority = "" }).ToString().Should().Be("scheme:/path");

            DocumentUri.Parse("scheme:/path").With(new DocumentUriComponents { Authority = "authority" })
                       .With(new DocumentUriComponents { Path = "" }).ToString().Should().Be("scheme://authority");

            DocumentUri.Parse("scheme:/path").With(new DocumentUriComponents { Authority = "" }).ToString().Should()
                       .Be("scheme:/path");
        }

        [Fact(DisplayName = "with, validation")]
        public void with_validation()
        {
            var uri = DocumentUri.Parse("foo:bar/path");
            Assert.Throws<UriFormatException>(() => uri.With(new DocumentUriComponents { Scheme = "fai:l" }));
            Assert.Throws<UriFormatException>(() => uri.With(new DocumentUriComponents { Scheme = "fäil" }));
            Assert.Throws<UriFormatException>(() => uri.With(new DocumentUriComponents { Authority = "fail" }));
            Assert.Throws<UriFormatException>(() => uri.With(new DocumentUriComponents { Path = "//fail" }));
        }

        [Fact(DisplayName = "parse")]
        public void Parse()
        {
            var value = DocumentUri.Parse("http:/api/files/test.me?t=1234");
            value.Scheme.Should().Be("http");
            value.Authority.Should().Be("");
            value.Path.Should().Be("/api/files/test.me");
            value.Query.Should().Be("t=1234");
            value.Fragment.Should().Be("");

            value = DocumentUri.Parse("http://api/files/test.me?t=1234");
            value.Scheme.Should().Be("http");
            value.Authority.Should().Be("api");
            value.Path.Should().Be("/files/test.me");
            value.Query.Should().Be("t=1234");
            value.Fragment.Should().Be("");

            value = DocumentUri.Parse("file:///c:/test/me");
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("");
            value.Path.Should().Be("/c:/test/me");
            value.Fragment.Should().Be("");
            value.Query.Should().Be("");
            value.GetFileSystemPath().Should().Be("c:\\test\\me");

            value = DocumentUri.Parse("file://shares/files/c%23/p.cs");
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("shares");
            value.Path.Should().Be("/files/c#/p.cs");
            value.Fragment.Should().Be("");
            value.Query.Should().Be("");
            value.GetFileSystemPath().Should().Be("\\\\shares\\files\\c#\\p.cs");

            value = DocumentUri.Parse(
                "file:///c:/Source/Z%C3%BCrich%20or%20Zurich%20(%CB%88zj%CA%8A%C9%99r%C9%AAk,/Code/resources/app/plugins/c%23/plugin.json"
            );
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("");
            value.Path.Should().Be(
                "/c:/Source/Zürich or Zurich (ˈzjʊərɪk,/Code/resources/app/plugins/c#/plugin.json"
            );
            value.Fragment.Should().Be("");
            value.Query.Should().Be("");

            value = DocumentUri.Parse("file:///c:/test %25/path");
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("");
            value.Path.Should().Be("/c:/test %/path");
            value.Fragment.Should().Be("");
            value.Query.Should().Be("");

            value = DocumentUri.Parse("inmemory:");
            value.Scheme.Should().Be("inmemory");
            value.Authority.Should().Be("");
            value.Path.Should().Be("");
            value.Query.Should().Be("");
            value.Fragment.Should().Be("");

            value = DocumentUri.Parse("foo:api/files/test");
            value.Scheme.Should().Be("foo");
            value.Authority.Should().Be("");
            value.Path.Should().Be("api/files/test");
            value.Query.Should().Be("");
            value.Fragment.Should().Be("");

            value = DocumentUri.Parse("file:?q");
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("");
            value.Path.Should().Be("/");
            value.Query.Should().Be("q");
            value.Fragment.Should().Be("");

            value = DocumentUri.Parse("file:#d");
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("");
            value.Path.Should().Be("/");
            value.Query.Should().Be("");
            value.Fragment.Should().Be("d");

            value = DocumentUri.Parse("f3ile:#d");
            value.Scheme.Should().Be("f3ile");
            value.Authority.Should().Be("");
            value.Path.Should().Be("");
            value.Query.Should().Be("");
            value.Fragment.Should().Be("d");

            value = DocumentUri.Parse("foo+bar:path");
            value.Scheme.Should().Be("foo+bar");
            value.Authority.Should().Be("");
            value.Path.Should().Be("path");
            value.Query.Should().Be("");
            value.Fragment.Should().Be("");

            value = DocumentUri.Parse("foo-bar:path");
            value.Scheme.Should().Be("foo-bar");
            value.Authority.Should().Be("");
            value.Path.Should().Be("path");
            value.Query.Should().Be("");
            value.Fragment.Should().Be("");

            value = DocumentUri.Parse("foo.bar:path");
            value.Scheme.Should().Be("foo.bar");
            value.Authority.Should().Be("");
            value.Path.Should().Be("path");
            value.Query.Should().Be("");
            value.Fragment.Should().Be("");
        }

        [Fact(DisplayName = "parse, disallow")]
        public void parse_disallow() => Assert.Throws<UriFormatException>(() => DocumentUri.Parse("file:////shares/files/p.cs"));

        [Fact(DisplayName = "URI#file, win-speciale")]
        public void URI_file__win_speciale()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var value = DocumentUri.File("c:\\test\\drive");
                value.Path.Should().Be("/c:/test/drive");
                value.ToString().Should().Be("file:///c:/test/drive");

                value = DocumentUri.File("\\\\shäres\\path\\c#\\plugin.json");
                value.Scheme.Should().Be("file");
                value.Authority.Should().Be("shäres");
                value.Path.Should().Be("/path/c#/plugin.json");
                value.Fragment.Should().Be("");
                value.Query.Should().Be("");
                value.ToString().Should().Be("file://sh%C3%A4res/path/c%23/plugin.json");

                value = DocumentUri.File("\\\\localhost\\c$\\GitDevelopment\\express");
                value.Scheme.Should().Be("file");
                value.Path.Should().Be("/c$/GitDevelopment/express");
                value.GetFileSystemPath().Should().Be("\\\\localhost\\c$\\GitDevelopment\\express");
                value.Query.Should().Be("");
                value.Fragment.Should().Be("");
                value.ToString().Should().Be("file://localhost/c%24/GitDevelopment/express");

                value = DocumentUri.File("c:\\test with %\\path");
                value.Path.Should().Be("/c:/test with %/path");
                value.ToString().Should().Be("file:///c:/test%20with%20%25/path");

                value = DocumentUri.File("c:\\test with %25\\path");
                value.Path.Should().Be("/c:/test with %25/path");
                value.ToString().Should().Be("file:///c:/test%20with%20%2525/path");

                value = DocumentUri.File("c:\\test with %25\\c#code");
                value.Path.Should().Be("/c:/test with %25/c#code");
                value.ToString().Should().Be("file:///c:/test%20with%20%2525/c%23code");

                value = DocumentUri.File("\\\\shares");
                value.Scheme.Should().Be("file");
                value.Authority.Should().Be("shares");
                value.Path.Should().Be("/"); // slash is always there

                value = DocumentUri.File("\\\\shares\\");
                value.Scheme.Should().Be("file");
                value.Authority.Should().Be("shares");
                value.Path.Should().Be("/");
            }
        }

        [Fact(DisplayName = "VSCode URI module\"s driveLetterPath regex is incorrect, #32961")]
        public void VSCode_URI_module_s_driveLetterPath_regex_is_incorrect__32961()
        {
            var uri = DocumentUri.Parse("file:///_:/path");
            uri.GetFileSystemPath().Should().Be("/_:/path");
        }

        [Fact(DisplayName = "URI#file, no path-is-uri check")]
        public void URI_file__no_path_is_uri_check()
        {
            // we don"t complain here
            var value = DocumentUri.File("file://path/to/file");
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("");
            value.Path.Should().Be("/file://path/to/file");
        }

        [Fact(DisplayName = "URI#file, always slash")]
        public void URI_file__always_slash()
        {
            var value = DocumentUri.File("a.file");
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("");
            value.Path.Should().Be("/a.file");
            value.ToString().Should().Be("file:///a.file");

            value = DocumentUri.Parse(value.ToString());
            value.Scheme.Should().Be("file");
            value.Authority.Should().Be("");
            value.Path.Should().Be("/a.file");
            value.ToString().Should().Be("file:///a.file");
        }

        [Fact(DisplayName = "URI.toString, only scheme and query")]
        public void URI_toString_only_scheme_and_query()
        {
            var value = DocumentUri.Parse("stuff:?qüery");
            value.ToString().Should().Be("stuff:?q%C3%BCery");
        }

        [Fact(DisplayName = "URI#toString, upper-case percent espaces")]
        public void URI_toString_upper_case_percent_espaces()
        {
            var value = DocumentUri.Parse("file://sh%c3%a4res/path");
            value.ToString().Should().Be("file://sh%C3%A4res/path");
        }

        [Fact(DisplayName = "URI#toString, lower-case windows drive letter")]
        public void URI_toString_lower_case_windows_drive_letter()
        {
            DocumentUri.Parse("untitled:c:/Users/jrieken/Code/abc.txt").ToString().Should().Be(
                "untitled:c:/Users/jrieken/Code/abc.txt"
            );
            DocumentUri.Parse("untitled:C:/Users/jrieken/Code/abc.txt").ToString().Should().Be(
                "untitled:c:/Users/jrieken/Code/abc.txt"
            );
        }

        [Fact(DisplayName = "URI#toString, escape all the bits")]
        public void URI_toString__escape_all_the_bits()
        {
            var value = DocumentUri.File("/Users/jrieken/Code/_samples/18500/Mödel + Other Thîngß/model.js");
            value.ToString().Should().Be(
                "file:///Users/jrieken/Code/_samples/18500/M%C3%B6del%20%2B%20Other%20Th%C3%AEng%C3%9F/model.js"
            );
        }

        [Fact(DisplayName = "URI#toString, don\"t encode port")]
        public void URI_toString_don_t_encode_port()
        {
            var value = DocumentUri.Parse("http://localhost:8080/far");
            value.ToString().Should().Be("http://localhost:8080/far");

            value = DocumentUri.From(
                new DocumentUriComponents
                    { Scheme = "http", Authority = "löcalhost:8080", Path = "/far", Query = null, Fragment = null }
            );
            value.ToString().Should().Be("http://l%C3%B6calhost:8080/far");
        }

        [Fact(DisplayName = "URI#toString, user information in authority")]
        public void URI_toString_user_information_in_authority()
        {
            var value = DocumentUri.Parse("http://foo:bar@localhost/far");
            value.ToString().Should().Be("http://foo:bar@localhost/far");

            value = DocumentUri.Parse("http://foo@localhost/far");
            value.ToString().Should().Be("http://foo@localhost/far");

            value = DocumentUri.Parse("http://foo:bAr@localhost:8080/far");
            value.ToString().Should().Be("http://foo:bAr@localhost:8080/far");

            value = DocumentUri.Parse("http://foo@localhost:8080/far");
            value.ToString().Should().Be("http://foo@localhost:8080/far");

            value = DocumentUri.From(
                new DocumentUriComponents
                    { Scheme = "http", Authority = "föö:bör@löcalhost:8080", Path = "/far", Query = null, Fragment = null }
            );
            value.ToString().Should().Be("http://f%C3%B6%C3%B6:b%C3%B6r@l%C3%B6calhost:8080/far");
        }

        [Fact(DisplayName = "correctFileUriToFilePath2")]
        public void CorrectFileUriToFilePath2()
        {
            Action<string, string> test = (input, expected) => {
                var value = DocumentUri.Parse(input);
                value.GetFileSystemPath().Should().Be(expected);
                var value2 = DocumentUri.File(value.GetFileSystemPath());
                value2.GetFileSystemPath().Should().Be(expected);
                value.ToString().Should().Be(value2.ToString());
            };

            test("file:///c:/alex.txt", "c:\\alex.txt");
            test(
                "file:///c:/Source/Z%C3%BCrich%20or%20Zurich%20(%CB%88zj%CA%8A%C9%99r%C9%AAk,/Code/resources/app/plugins",
                "c:\\Source\\Zürich or Zurich (ˈzjʊərɪk,\\Code\\resources\\app\\plugins"
            );
            test(
                "file://monacotools/folder/isi.txt",
                "\\\\monacotools\\folder\\isi.txt"
            );
            test(
                "file://monacotools1/certificates/SSL/",
                "\\\\monacotools1\\certificates\\SSL\\"
            );
        }

        [Fact(DisplayName = "URI - http, query & toString")]
        public void URI___http__query___toString()
        {
            var uri = DocumentUri.Parse("https://go.microsoft.com/fwlink/?LinkId=518008");
            uri.Query.Should().Be("LinkId=518008");
            uri.ToUnencodedString().Should().Be("https://go.microsoft.com/fwlink/?LinkId=518008");
            uri.ToString().Should().Be("https://go.microsoft.com/fwlink/?LinkId%3D518008");

            var uri2 = DocumentUri.Parse(uri.ToString());
            uri2.Query.Should().Be("LinkId=518008");
            uri2.Query.Should().Be(uri.Query);

            uri = DocumentUri.Parse("https://go.microsoft.com/fwlink/?LinkId=518008&foö&ké¥=üü");
            uri.Query.Should().Be("LinkId=518008&foö&ké¥=üü");
            uri.ToUnencodedString().Should().Be("https://go.microsoft.com/fwlink/?LinkId=518008&foö&ké¥=üü");
            uri.ToString().Should().Be(
                "https://go.microsoft.com/fwlink/?LinkId%3D518008%26fo%C3%B6%26k%C3%A9%C2%A5%3D%C3%BC%C3%BC"
            );

            uri2 = DocumentUri.Parse(uri.ToString());
            uri2.Query.Should().Be("LinkId=518008&foö&ké¥=üü");
            uri2.Query.Should().Be(uri.Query);

            // #24849
            uri = DocumentUri.Parse("https://twitter.com/search?src=typd&q=%23tag");
            uri.ToUnencodedString().Should().Be("https://twitter.com/search?src=typd&q=%23tag");
        }


        [Fact(DisplayName = "class URI cannot represent relative file paths #34449")]
        public void class_URI_cannot_represent_relative_file_paths__34449()
        {
            var path = "/foo/bar";
            DocumentUri.File(path).Path.Should().Be(path);
            path = "foo/bar";
            DocumentUri.File(path).Path.Should().Be("/foo/bar");
            path = "./foo/bar";
            DocumentUri.File(path).Path.Should().Be("/./foo/bar"); // missing normalization

            var fileUri1 = DocumentUri.Parse("file:foo/bar");
            fileUri1.Path.Should().Be("/foo/bar");
            fileUri1.Authority.Should().Be("");
            var uri = fileUri1.ToString();
            uri.Should().Be("file:///foo/bar");
            var fileUri2 = DocumentUri.Parse(uri);
            fileUri2.Path.Should().Be("/foo/bar");
            fileUri2.Authority.Should().Be("");
        }

        [Fact(DisplayName = "Ctrl click to follow hash query param url gets urlencoded #49628")]
        public void Ctrl_click_to_follow_hash_query_param_url_gets_urlencoded_49628()
        {
            var input = "http://localhost:3000/#/foo?bar=baz";
            var uri = DocumentUri.Parse(input);
            uri.ToUnencodedString().Should().Be(input);

            input = "http://localhost:3000/foo?bar=baz";
            uri = DocumentUri.Parse(input);
            uri.ToUnencodedString().Should().Be(input);
        }

        [Fact(DisplayName = "Unable to open \"%A0.txt\": URI malformed #76506")]
        public void Unable_to_open_URI_malformed_76506()
        {
            var uri = DocumentUri.File("/foo/%A0.txt");
            var uri2 = DocumentUri.Parse(uri.ToString());
            uri.Scheme.Should().Be(uri2.Scheme);
            uri.Path.Should().Be(uri2.Path);

            uri = DocumentUri.File("/foo/%2e.txt");
            uri2 = DocumentUri.Parse(uri.ToString());
            uri.Scheme.Should().Be(uri2.Scheme);
            uri.Path.Should().Be(uri2.Path);
        }

        [Fact(DisplayName = "Unable to open \"%A0.txt\": URI malformed #76506")]
        public void Unable_to_open_URI_malformed_76506_2()
        {
            DocumentUri.Parse("file://some/%.txt").ToString().Should().Be("file://some/%25.txt");
            DocumentUri.Parse("file://some/%A0.txt").ToString().Should().Be("file://some/%25A0.txt");
        }
    }
}
