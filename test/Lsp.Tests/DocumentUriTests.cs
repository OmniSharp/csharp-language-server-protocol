using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
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

        [SkippableTheory]
        [ClassData(typeof(DocumentUriTestData.FileSystemToFileSystem))]
        public void Should_Handle_VSCode_Style_Uris(string uri, string expected)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.NormalizePath(uri).Should().Be(expected);
        }

        [SkippableTheory]
        [ClassData(typeof(DocumentUriTestData.FileUriToFileUri))]
        public void Should_Handle_VSCode_Style_Paths(string uri, string expected)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.NormalizeUri(uri).Should().Be(expected);
        }

        [SkippableTheory]
        [ClassData(typeof(DocumentUriTestData.FileSystemToFileUri))]
        public void Should_Normalize_VSCode_Style_FileSystem_Paths(string uri, string expected)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.FromFileSystemPath(uri).Should().Be(expected);
        }

        [SkippableTheory]
        [ClassData(typeof(DocumentUriTestData.FileUriToFileSystem))]
        public void Should_Normalize_VSCode_Style_Uris(Uri uri, string expected)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.GetFileSystemPath(uri).Should().Be(expected);
        }
    }

    public static class DocumentUriTestData
    {
        public class FileUriToFileUri : IEnumerable<object[]>
        {
            private const string Path = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private IEnumerable<(string, string)> AddPaths(string path)
            {
                yield return (path.Replace("c:", "c%3A"), path);
                yield return (path.Replace("c:", "c%3a"), path);
                yield return (path, path);
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(Path))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(Path.Replace("Namespace", "Пространствоимен")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(Path.Replace("Namespace", "汉字漢字")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(Path.Replace("Namespace", "のはでした")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(Path.Replace("Namespace", "コンサート")))
                    yield return new object[] {source, destination};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class FileSystemToFileSystem : IEnumerable<object[]>
        {
            private const string Path = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";

            private IEnumerable<(string, string)> AddPaths(string path)
            {
                yield return (path.Replace("c:", "c%3A"), path);
                yield return (path.Replace("c:", "c%3a"), path);
                yield return (path, path);
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(Path))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(Path.Replace("Namespace", "Пространствоимен")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(Path.Replace("Namespace", "汉字漢字")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(Path.Replace("Namespace", "のはでした")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(Path.Replace("Namespace", "コンサート")))
                    yield return new object[] {source, destination};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class FileSystemToFileUri : IEnumerable<object[]>
        {
            private const string SourcePath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";
            private const string DestinationPath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(SourcePath, DestinationPath))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(SourcePath.Replace("Namespace", "Пространствоимен"),
                    DestinationPath.Replace("Namespace", "Пространствоимен")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(SourcePath.Replace("Namespace", "汉字漢字"),
                    DestinationPath.Replace("Namespace", "汉字漢字")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(SourcePath.Replace("Namespace", "のはでした"),
                    DestinationPath.Replace("Namespace", "のはでした")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(SourcePath.Replace("Namespace", "コンサート"),
                    DestinationPath.Replace("Namespace", "コンサート")))
                    yield return new object[] {source, destination};
            }

            private IEnumerable<(string, string)> AddPaths(string source, string destination)
            {
                yield return (source.Replace("c:", "c%3A"), destination);
                yield return (source.Replace("c:", "c%3a"), destination);
                yield return (source, destination);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class FileUriToFileSystem : IEnumerable<object[]>
        {
            private const string SourcePath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";
            private const string DestinationPath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";

            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var (source, destination) in AddPaths(SourcePath, DestinationPath))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(SourcePath.Replace("Namespace", "Пространствоимен"),
                    DestinationPath.Replace("Namespace", "Пространствоимен")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(SourcePath.Replace("Namespace", "汉字漢字"),
                    DestinationPath.Replace("Namespace", "汉字漢字")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(SourcePath.Replace("Namespace", "のはでした"),
                    DestinationPath.Replace("Namespace", "のはでした")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(SourcePath.Replace("Namespace", "コンサート"),
                    DestinationPath.Replace("Namespace", "コンサート")))
                    yield return new object[] {source, destination};
            }

            private IEnumerable<(Uri, string)> AddPaths(string source, string destination)
            {
                yield return (new Uri(source), destination);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }

    public class AbsoluteUriConverterTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AbsoluteUriConverterTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        [SkippableTheory]
        [ClassData(typeof(DocumentUriTestData.FileUriToFileUri))]
        public void Should_Deserialize_VSCode_Style_Uris(string uri, string expected)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            var serializer = new JsonSerializerSettings() {
                Converters = {new AbsoluteUriConverter()}
            };
            JsonConvert.DeserializeObject<Uri>($"\"{uri}\"", serializer).Should().Be(expected);
        }

        [SkippableTheory]
        [ClassData(typeof(DocumentUriTestData.FileUriToFileUri))]
        public void Should_Serialize_VSCode_Style_Uris(string uri, string expected)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            var serializer = new JsonSerializerSettings() {
                Converters = {new AbsoluteUriConverter()}
            };
            JsonConvert.SerializeObject(new Uri(uri), serializer).Trim('"').Should().Be(expected);
        }
    }
}
