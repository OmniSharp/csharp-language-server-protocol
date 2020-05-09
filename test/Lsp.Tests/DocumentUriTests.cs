using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
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
                    WindowsPathAlt.Replace("Namespace", "Пространствоимен"),
                    WindowsPathAlt.Replace("Namespace", "汉字漢字"),
                    WindowsPathAlt.Replace("Namespace", "のはでした"),
                    WindowsPathAlt.Replace("Namespace", "コンサート")
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
                    WindowsPath.Replace("Namespace", "Пространствоимен"),
                    WindowsPath.Replace("Namespace", "汉字漢字"),
                    WindowsPath.Replace("Namespace", "のはでした"),
                    WindowsPath.Replace("Namespace", "コンサート")
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
                    UncPath.Replace("Namespace", "Пространствоимен"),
                    UncPath.Replace("Namespace", "汉字漢字"),
                    UncPath.Replace("Namespace", "のはでした"),
                    UncPath.Replace("Namespace", "コンサート")
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
                    UnixPath.Replace("Namespace", "Пространствоимен"),
                    UnixPath.Replace("Namespace", "汉字漢字"),
                    UnixPath.Replace("Namespace", "のはでした"),
                    UnixPath.Replace("Namespace", "コンサート")
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
                            WindowsDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "汉字漢字"),
                            WindowsDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "のはでした"),
                            WindowsDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "コンサート"),
                            WindowsDestinationPath.Replace("Namespace", "コンサート")))
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
                            UncDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "汉字漢字"),
                            UncDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "のはでした"),
                            UncDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "コンサート"),
                            UncDestinationPath.Replace("Namespace", "コンサート")))
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
                            UnixDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "汉字漢字"),
                            UnixDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "のはでした"),
                            UnixDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "コンサート"),
                            UnixDestinationPath.Replace("Namespace", "コンサート")))
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
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "Пространствоимен"),
                            WindowsDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "汉字漢字"),
                            WindowsDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "のはでした"),
                            WindowsDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(WindowsSourcePath.Replace("Namespace", "コンサート"),
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
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "Пространствоимен"),
                            UncDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "汉字漢字"),
                            UncDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "のはでした"),
                            UncDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(UncSourcePath.Replace("Namespace", "コンサート"),
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
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "Пространствоимен"),
                            UnixDestinationPath.Replace("Namespace", "Пространствоимен")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "汉字漢字"),
                            UnixDestinationPath.Replace("Namespace", "汉字漢字")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "のはでした"),
                            UnixDestinationPath.Replace("Namespace", "のはでした")))
                        .Concat(AddPaths(UnixSourcePath.Replace("Namespace", "コンサート"),
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
                foreach (var path in paths)
                {
                    yield return (path.Replace("c:", "c%3A"), path);
                    yield return (path.Replace("c:", "c%3a"), path);
                    yield return (path, path);
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
