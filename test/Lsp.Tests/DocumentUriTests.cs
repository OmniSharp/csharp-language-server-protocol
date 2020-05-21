using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests
{
    public class DocumentUriTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DocumentUriTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(WindowsFileSystemPaths))]
        public void Should_Handle_Windows_File_System_Paths(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).ToString().Should().Be(expected);
        }

        public class WindowsFileSystemPaths : BaseSourceDestination
        {
            private const string WindowsPath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\{0}s.cs";

            public WindowsFileSystemPaths() : base(WindowsPath, "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs") { }
        }

        [Theory]
        [ClassData(typeof(UncFileSystemPaths))]
        public void Should_Handle_Unc_File_System_Paths(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).ToString().Should().Be(expected);
        }

        public class UncFileSystemPaths : BaseSourceDestination
        {
            private const string UncPath = "\\\\myserver\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\{0}s.cs";

            public UncFileSystemPaths() : base(UncPath, "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs") { }
        }

        [Theory]
        [ClassData(typeof(UnixFileSystemPaths))]
        public void Should_Handle_Unix_File_System_Paths(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).ToString().Should().Be(expected);
        }

        public class UnixFileSystemPaths : BaseSourceDestination
        {
            private const string UnixPath = "/usr/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public UnixFileSystemPaths() : base(UnixPath, "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs") { }
        }

        [Theory]
        [ClassData(typeof(WindowsPathStringUris))]
        public void Should_Handle_Windows_String_Uris(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.Parse(uri).ToString().Should().Be(expected);
        }

        public class WindowsPathStringUris : BaseSourceDestination
        {
            private const string WindowsPath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public WindowsPathStringUris() : base(WindowsPath, WindowsPath) { }
        }

        [Theory]
        [ClassData(typeof(WindowsPathAltStringUris))]
        public void Should_Handle_Windows_Alt_String_Uris(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.Parse(uri).ToString().Should().Be(expected);
        }

        public class WindowsPathAltStringUris : BaseSourceDestination
        {
            private const string WindowsPathAlt = "file://c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public WindowsPathAltStringUris() : base(WindowsPathAlt, WindowsPathAlt) { }
        }

        [Theory]
        [ClassData(typeof(UncPathStringUris))]
        public void Should_Handle_Unc_String_Uris(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.Parse(uri).ToString().Should().Be(expected);
        }

        public class UncPathStringUris : BaseSourceDestination
        {
            private const string UncPath = "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public UncPathStringUris() : base(UncPath, UncPath) { }
        }

        [Theory]
        [ClassData(typeof(UnixPathStringUris))]
        public void Should_Handle_Unix_String_Uris(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.Parse(uri).ToString().Should().Be(expected);
        }

        public class UnixPathStringUris : BaseSourceDestination
        {
            private const string UnixPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public UnixPathStringUris() : base(UnixPath, UnixPath) { }
        }

        [Theory]
        [ClassData(typeof(ResourceStringUris))]
        [ClassData(typeof(ResourceStringUrisWithPaths))]
        public void Should_Handle_Resource_String_Uris(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).ToString().Should().Be(expected);
        }

        public class ResourceStringUris : BaseSourceDestination
        {
            private const string ResourcePath = "untitled:{0}-1";
            public ResourceStringUris() : base(ResourcePath, ResourcePath) { }
        }

        public class ResourceStringUrisWithPaths : BaseSourceDestination
        {
            private const string ResourcePathWithPath = "untitled:{0}-1/some/path";
            public ResourceStringUrisWithPaths() : base(ResourcePathWithPath, ResourcePathWithPath) { }
        }

        [Theory]
        [ClassData(typeof(WindowsPathUris))]
        public void Should_Handle_Windows_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).ToString().Should().Be(expected);
        }

        public class WindowsPathUris : UriSourceDestination
        {
            private const string WindowsPath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";
            public WindowsPathUris() : base(WindowsPath, WindowsPath) { }
        }

        [Theory]
        [ClassData(typeof(UncPathUris))]
        public void Should_Handle_Unc_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).ToString().Should().Be(expected);
        }

        public class UncPathUris : UriSourceDestination
        {
            private const string UncPath = "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";
            public UncPathUris() : base(UncPath, UncPath) { }
        }

        [Theory]
        [ClassData(typeof(UnixPathUris))]
        public void Should_Handle_Unix_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).ToString().Should().Be(expected);
        }

        public class UnixPathUris : UriSourceDestination
        {
            private const string UnixPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";
            public UnixPathUris() : base(UnixPath, UnixPath) { }
        }

        [Theory]
        [ClassData(typeof(WindowsFileSystemToFileUri))]
        public void Should_Normalize_Windows_FileSystem_Paths(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.FromFileSystemPath(uri).ToString().Should().Be(expected);
        }

        public class WindowsFileSystemToFileUri : BaseSourceDestination
        {
            private const string WindowsSourcePath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\{0}s.cs";
            private const string WindowsDestinationPath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public WindowsFileSystemToFileUri() : base(WindowsSourcePath, WindowsDestinationPath) { }
        }

        [Theory]
        [ClassData(typeof(UncFileSystemToFileUri))]
        public void Should_Normalize_Unc_FileSystem_Paths(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.FromFileSystemPath(uri).ToString().Should().Be(expected);
        }

        public class UncFileSystemToFileUri : BaseSourceDestination
        {
            private const string UncSourcePath = "\\\\myserver\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\{0}s.cs";
            private const string UncDestinationPath = "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public UncFileSystemToFileUri() : base(UncSourcePath, UncDestinationPath) { }
        }

        [Theory]
        [ClassData(typeof(UnixFileSystemToFileUri))]
        public void Should_Normalize_Unix_FileSystem_Paths(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.FromFileSystemPath(uri).ToString().Should().Be(expected);
        }

        public class UnixFileSystemToFileUri : BaseSourceDestination
        {
            private const string UnixSourcePath = "/usr/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";
            private const string UnixDestinationPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public UnixFileSystemToFileUri() : base(UnixSourcePath, UnixDestinationPath) { }
        }

        [Theory]
        [ClassData(typeof(WindowsFileUriToFileSystem))]
        public void Should_Normalize_Windows_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.GetFileSystemPath(uri).Should().Be(expected);
        }

        public class WindowsFileUriToFileSystem : UriSourceDestination
        {
            private const string WindowsSourcePath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";
            private const string WindowsDestinationPath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\{0}s.cs";

            public WindowsFileUriToFileSystem() : base(WindowsSourcePath, WindowsDestinationPath, false) { }
        }

        [Theory]
        [ClassData(typeof(UncFileUriToFileSystem))]
        public void Should_Normalize_Unc_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.GetFileSystemPath(uri).Should().Be(expected);
        }

        public class UncFileUriToFileSystem : UriSourceDestination
        {
            private const string UncSourcePath = "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";
            private const string UncDestinationPath = "\\\\myserver\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\{0}s.cs";

            public UncFileUriToFileSystem() : base(UncSourcePath, UncDestinationPath, false) { }
        }

        [Theory]
        [ClassData(typeof(UnixFileUriToFileSystem))]
        public void Should_Normalize_Unix_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.GetFileSystemPath(uri).Should().Be(expected);
        }

        public class UnixFileUriToFileSystem : UriSourceDestination
        {
            private const string UnixSourcePath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";
            private const string UnixDestinationPath = "/usr/mb/src/gh/Cake.Json/src/Cake.Json/{0}s.cs";

            public UnixFileUriToFileSystem() : base(UnixSourcePath, UnixDestinationPath, false) { }
        }

        private static string[] EncodedStrings = new[] {"Namespace", "Пространствоимен", "汉字漢字", "のはでした", "コンサート"};

        public class UriSourceDestination : IEnumerable<object[]>
        {
            private readonly string _sourceFormat;
            private readonly string _destinationFormat;
            private readonly bool _encode;

            protected UriSourceDestination(string sourceFormat, string destinationFormat, bool encode = true)
            {
                _sourceFormat = sourceFormat;
                _destinationFormat = destinationFormat;
                _encode = encode;
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var data in EncodedStrings)
                {
                    var encodedData = _encode ? Uri.EscapeDataString(data) : data;
                    if (_sourceFormat.Replace("c:", "c%3A") != string.Format(_sourceFormat, data))
                        yield return new object[] {
                            new Uri(string.Format(_sourceFormat, data).Replace("c:", "c%3A")),
                            string.Format(_destinationFormat, encodedData)
                        };
                    if (_sourceFormat.Replace("c:", "c%3a") != string.Format(_sourceFormat, data))
                        yield return new object[] {
                            new Uri(string.Format(_sourceFormat, data).Replace("c:", "c%3a")),
                            string.Format(_destinationFormat, encodedData)
                        };
                    yield return new object[] {
                        new Uri(string.Format(_sourceFormat, data)),
                        string.Format(_destinationFormat, encodedData)
                    };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public abstract class BaseSourceDestination : IEnumerable<object[]>
        {
            private readonly string _sourceFormat;
            private readonly string _destinationFormat;

            protected BaseSourceDestination(string sourceFormat, string destinationFormat)
            {
                _sourceFormat = sourceFormat;
                _destinationFormat = destinationFormat;
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var data in EncodedStrings)
                {
                    if (_sourceFormat.Replace("c:", "c%3A") != string.Format(_sourceFormat, data))
                        yield return new object[] {
                            string.Format(_sourceFormat, data).Replace("c:", "c%3A"),
                            string.Format(_destinationFormat, Uri.EscapeDataString(data))
                        };
                    if (_sourceFormat.Replace("c:", "c%3a") != string.Format(_sourceFormat, data))
                        yield return new object[] {
                            string.Format(_sourceFormat, data).Replace("c:", "c%3a"),
                            string.Format(_destinationFormat, Uri.EscapeDataString(data))
                        };
                    yield return new object[] {
                        string.Format(_sourceFormat, data),
                        string.Format(_destinationFormat, Uri.EscapeDataString(data))
                    };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
