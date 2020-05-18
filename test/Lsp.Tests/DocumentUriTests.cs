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
        public void Should_Handle_Windows_File_System_Paths(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        public class WindowsFileSystemPaths : BaseSingle
        {
            private const string WindowsPath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    WindowsPath,
                    WindowsPath.Replace("Namespace", "Пространствоимен"),
                    WindowsPath.Replace("Namespace", "汉字漢字"),
                    WindowsPath.Replace("Namespace", "のはでした"),
                    WindowsPath.Replace("Namespace", "コンサート")
                ))
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(UncFileSystemPaths))]
        public void Should_Handle_Unc_File_System_Paths(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        public class UncFileSystemPaths : BaseSingle
        {
            private const string UncPath = "\\\\myserver\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    UncPath,
                    UncPath.Replace("Namespace", "Пространствоимен"),
                    UncPath.Replace("Namespace", "汉字漢字"),
                    UncPath.Replace("Namespace", "のはでした"),
                    UncPath.Replace("Namespace", "コンサート")
                ))
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(UnixFileSystemPaths))]
        public void Should_Handle_Unix_File_System_Paths(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        public class UnixFileSystemPaths : BaseSingle
        {
            private const string UnixPath = "/usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    UnixPath,
                    UnixPath.Replace("Namespace", "Пространствоимен"),
                    UnixPath.Replace("Namespace", "汉字漢字"),
                    UnixPath.Replace("Namespace", "のはでした"),
                    UnixPath.Replace("Namespace", "コンサート")
                ))
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(WindowsPathStringUris))]
        public void Should_Handle_Windows_String_Uris(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        public class WindowsPathStringUris : BaseSingle
        {
            private const string WindowsPath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                if (true)
                {
                    foreach (var (source, destination) in AddPaths(
                        WindowsPath,
                        // Пространствоимен
                        WindowsPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                        // 汉字漢字
                        WindowsPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                        // のはでした
                        WindowsPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                        // コンサート
                        WindowsPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")
                    ))
                    {
                        yield return new object[] {source, destination};
                    }
                }
            }
        }

        [Theory]
        [ClassData(typeof(WindowsPathAltStringUris))]
        public void Should_Handle_Windows_Alt_String_Uris(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        public class WindowsPathAltStringUris : BaseSingle
        {
            private const string WindowsPathAlt = "file://c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    WindowsPathAlt,
                    // Пространствоимен
                    WindowsPathAlt.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                    // 汉字漢字
                    WindowsPathAlt.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                    // のはでした
                    WindowsPathAlt.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                    // コンサート
                    WindowsPathAlt.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")
                ))
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(UncPathStringUris))]
        public void Should_Handle_Unc_String_Uris(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        public class UncPathStringUris : BaseSingle
        {
            private const string UncPath = "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    UncPath,
                    // Пространствоимен
                    UncPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                    // 汉字漢字
                    UncPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                    // のはでした
                    UncPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                    // コンサート
                    UncPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")
                ))
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(UnixPathStringUris))]
        public void Should_Handle_Unix_String_Uris(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        public class UnixPathStringUris : BaseSingle
        {
            private const string UnixPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    UnixPath,
                    // Пространствоимен
                    UnixPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                    // 汉字漢字
                    UnixPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                    // のはでした
                    UnixPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                    // コンサート
                    UnixPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")
                ))
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(ResourceStringUris))]
        public void Should_Handle_Resource_String_Uris(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).ToString().Should().Be(expected);
        }

        public class ResourceStringUris : IEnumerable<object[]>
        {
            private const string ResourcePath = "untitled:Untitled-1";
            private const string ResourcePathWithPath = "untitled:Untitled-1/some/path";

            protected IEnumerable<(string, string)> AddPaths(params string[] paths)
            {
                foreach (var path in paths)
                {
                    yield return (path.Replace("c:", "c%3A"), path);
                    yield return (path.Replace("c:", "c%3a"), path);
                    yield return (path, path);
                }
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    ResourcePath,
                    // Пространствоимен
                    ResourcePath.Replace("Untitled", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                    // 汉字漢字
                    ResourcePath.Replace("Untitled", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                    // のはでした
                    ResourcePath.Replace("Untitled", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                    // コンサート
                    ResourcePath.Replace("Untitled", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88"),
                    ResourcePathWithPath,
                    // Пространствоимен
                    ResourcePathWithPath.Replace("Untitled", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                    // 汉字漢字
                    ResourcePathWithPath.Replace("Untitled", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                    // のはでした
                    ResourcePathWithPath.Replace("Untitled", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                    // コンサート
                    ResourcePathWithPath.Replace("Untitled", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")
                ))
                {
                    yield return new object[] {source, destination};
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(WindowsPathUris))]
        public void Should_Handle_Windows_Uris(Uri uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).Should().Be(expected);
        }

        public class WindowsPathUris : BaseSingle
        {
            private const string WindowsPath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    WindowsPath,
                    // Пространствоимен
                    WindowsPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                    // 汉字漢字
                    WindowsPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                    // のはでした
                    WindowsPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                    // コンサート
                    WindowsPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")
                ))
                {
                    yield return new object[] {new Uri(source), destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(UncPathUris))]
        public void Should_Handle_Unc_Uris(Uri uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).Should().Be(expected);
        }

        public class UncPathUris : BaseSingle
        {
            private const string UncPath = "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    UncPath,
                    // Пространствоимен
                    UncPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                    // 汉字漢字
                    UncPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                    // のはでした
                    UncPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                    // コンサート
                    UncPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")
                ))
                {
                    yield return new object[] {new Uri(source), destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(UnixPathUris))]
        public void Should_Handle_Unix_Uris(Uri uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).Should().Be(expected);
        }

        public class UnixPathUris : BaseSingle
        {
            private const string UnixPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(
                    UnixPath,
                    // Пространствоимен
                    UnixPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                    // 汉字漢字
                    UnixPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                    // のはでした
                    UnixPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                    // コンサート
                    UnixPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")
                ))
                {
                    yield return new object[] {new Uri(source), destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(WindowsFileSystemToFileUri))]
        public void Should_Normalize_Windows_FileSystem_Paths(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.FromFileSystemPath(uri).Should().Be(expected);
        }

        public class WindowsFileSystemToFileUri : BaseSourceDestination
        {
            private const string WindowsSourcePath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";

            private const string WindowsDestinationPath =
                "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in
                    AddPaths(WindowsSourcePath, WindowsDestinationPath)
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "Пространствоимен"),
                            WindowsDestinationPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "汉字漢字"),
                            WindowsDestinationPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "のはでした"),
                            WindowsDestinationPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "コンサート"),
                            WindowsDestinationPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")))
                )
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(UncFileSystemToFileUri))]
        public void Should_Normalize_Unc_FileSystem_Paths(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.FromFileSystemPath(uri).Should().Be(expected);
        }

        public class UncFileSystemToFileUri : BaseSourceDestination
        {
            private const string UncSourcePath =
                "\\\\myserver\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";

            private const string UncDestinationPath =
                "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in
                    AddPaths(UncSourcePath, UncDestinationPath)
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "Пространствоимен"),
                            UncDestinationPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "汉字漢字"),
                            UncDestinationPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "のはでした"),
                            UncDestinationPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "コンサート"),
                            UncDestinationPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")))
                )
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(UnixFileSystemToFileUri))]
        public void Should_Normalize_Unix_FileSystem_Paths(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.FromFileSystemPath(uri).Should().Be(expected);
        }

        public class UnixFileSystemToFileUri : BaseSourceDestination
        {
            private const string UnixSourcePath = "/usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";
            private const string UnixDestinationPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in
                    AddPaths(UnixSourcePath, UnixDestinationPath)
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "Пространствоимен"),
                            UnixDestinationPath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "汉字漢字"),
                            UnixDestinationPath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "のはでした"),
                            UnixDestinationPath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "コンサート"),
                            UnixDestinationPath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88")))
                )
                {
                    yield return new object[] {source, destination};
                }
            }
        }

        [Theory]
        [ClassData(typeof(WindowsFileUriToFileSystem))]
        public void Should_Normalize_Windows_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.GetFileSystemPath(uri).Should().Be(expected);
        }

        public class WindowsFileUriToFileSystem : BaseSourceDestination
        {
            private const string WindowsSourcePath =
                "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private const string WindowsDestinationPath =
                "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in
                    AddPaths(WindowsSourcePath, WindowsDestinationPath)
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                            WindowsDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                            WindowsDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                            WindowsDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88"),
                            WindowsDestinationPath.Replace("Namespace", "コンサート")))
                )
                {
                    yield return new object[] {source, destination};
                }
            }

            private IEnumerable<(Uri, string)> AddPaths(string source, string destination)
            {
                yield return (new Uri(source), destination);
            }
        }

        [Theory]
        [ClassData(typeof(UncFileUriToFileSystem))]
        public void Should_Normalize_Unc_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.GetFileSystemPath(uri).Should().Be(expected);
        }

        public class UncFileUriToFileSystem : BaseSourceDestination
        {
            private const string UncSourcePath =
                "file://myserver/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private const string UncDestinationPath =
                "\\\\myserver\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in
                    AddPaths(UncSourcePath, UncDestinationPath)
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                            UncDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                            UncDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                            UncDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88"),
                            UncDestinationPath.Replace("Namespace", "コンサート")))
                )
                {
                    yield return new object[] {source, destination};
                }
            }

            private IEnumerable<(Uri, string)> AddPaths(string source, string destination)
            {
                yield return (new Uri(source), destination);
            }
        }

        [Theory]
        [ClassData(typeof(UnixFileUriToFileSystem))]
        public void Should_Normalize_Unix_Uris(Uri uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.GetFileSystemPath(uri).Should().Be(expected);
        }

        public class UnixFileUriToFileSystem : BaseSourceDestination
        {
            private const string UnixSourcePath =
                "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private const string UnixDestinationPath =
                "/usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public override IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in
                    AddPaths(UnixSourcePath, UnixDestinationPath)
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%80%D0%B0%D0%BD%D1%81%D1%82%D0%B2%D0%BE%D0%B8%D0%BC%D0%B5%D0%BD"),
                            UnixDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "%E6%B1%89%E5%AD%97%E6%BC%A2%E5%AD%97"),
                            UnixDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "%E3%81%AE%E3%81%AF%E3%81%A7%E3%81%97%E3%81%9F"),
                            UnixDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "%E3%82%B3%E3%83%B3%E3%82%B5%E3%83%BC%E3%83%88"),
                            UnixDestinationPath.Replace("Namespace", "コンサート")))
                )
                {
                    yield return new object[] {source, destination};
                }
            }

            private IEnumerable<(Uri, string)> AddPaths(string source, string destination)
            {
                yield return (new Uri(source), destination);
            }
        }


        public abstract class BaseSingle : IEnumerable<object[]>
        {
            protected IEnumerable<(string, DocumentUri)> AddPaths(params string[] paths)
            {
                foreach (var expectedPath in paths)
                {
                    if (expectedPath.Replace("c:", "c%3A") != expectedPath)
                        yield return (expectedPath.Replace("c:", "c%3A"), expectedPath);
                    if (expectedPath.Replace("c:", "c%3a") != expectedPath)
                        yield return (expectedPath.Replace("c:", "c%3a"), expectedPath);
                    yield return (expectedPath, expectedPath);
                }
            }

            public abstract IEnumerator<object[]> GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public abstract class BaseSourceDestination : IEnumerable<object[]>
        {
            public abstract IEnumerator<object[]> GetEnumerator();

            protected IEnumerable<(string, DocumentUri)> AddPaths(string source, DocumentUri destination)
            {
                yield return (source.Replace("c:", "c%3A"), destination);
                yield return (source.Replace("c:", "c%3a"), destination);
                yield return (source, destination);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
