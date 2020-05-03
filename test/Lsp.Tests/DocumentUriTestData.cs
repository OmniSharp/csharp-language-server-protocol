using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Lsp.Tests
{
    public static class DocumentUriTestData
    {
        public class StringUris : IEnumerable<object[]>
        {
            private const string WindowsPath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";
            private const string UnixPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private IEnumerable<(string, DocumentUri)> AddPaths(string path)
            {
                yield return (path.Replace("c:", "c%3A"), path);
                yield return (path.Replace("c:", "c%3a"), path);
                yield return (path, path);
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    foreach (var (source, destination) in AddPaths(WindowsPath))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsPath.Replace("Namespace", "Пространствоимен")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "汉字漢字")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "のはでした")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "コンサート")))
                        yield return new object[] {source, destination};
                }

                foreach (var (source, destination) in AddPaths(UnixPath))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(
                    UnixPath.Replace("Namespace", "Пространствоимен")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "汉字漢字")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "のはでした")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "コンサート")))
                    yield return new object[] {source, destination};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class Uris : IEnumerable<object[]>
        {
            private const string WindowsPath = "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";
            private const string UnixPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private IEnumerable<(Uri, DocumentUri)> AddPaths(string path)
            {
                yield return (new Uri(path.Replace("c:", "c%3A")), path);
                yield return (new Uri(path.Replace("c:", "c%3a")), path);
                yield return (new Uri(path), path);
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    foreach (var (source, destination) in AddPaths(WindowsPath))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "Пространствоимен"))
                    )
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "汉字漢字")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "のはでした")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "コンサート")))
                        yield return new object[] {source, destination};
                }

                foreach (var (source, destination) in AddPaths(UnixPath))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "Пространствоимен")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "汉字漢字")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "のはでした")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "コンサート")))
                    yield return new object[] {source, destination};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class FileSystemPaths : IEnumerable<object[]>
        {
            private const string WindowsPath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";
            private const string UnixPath = "/usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private IEnumerable<(string, DocumentUri)> AddPaths(string path)
            {
                yield return (path.Replace("c:", "c%3A"), path);
                yield return (path.Replace("c:", "c%3a"), path);
                yield return (path, path);
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    foreach (var (source, destination) in AddPaths(WindowsPath))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "Пространствоимен"))
                    )
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "汉字漢字")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "のはでした")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(WindowsPath.Replace("Namespace", "コンサート")))
                        yield return new object[] {source, destination};
                }
                else
                {
                    foreach (var (source, destination) in AddPaths(UnixPath))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "Пространствоимен")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "汉字漢字")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "のはでした")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(UnixPath.Replace("Namespace", "コンサート")))
                        yield return new object[] {source, destination};
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class FileSystemToFileUri : IEnumerable<object[]>
        {
            private const string WindowsSourcePath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";
            private const string UnixSourcePath = "/usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private const string WindowsDestinationPath =
                "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private const string UnixDestinationPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public IEnumerator<object[]> GetEnumerator()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath,
                        WindowsDestinationPath))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath.Replace("Namespace", "Пространствоимен"),
                        WindowsDestinationPath.Replace("Namespace", "Пространствоимен"))
                    )
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath.Replace("Namespace", "汉字漢字"),
                        WindowsDestinationPath.Replace("Namespace", "汉字漢字"))
                    )
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath.Replace("Namespace", "のはでした"),
                        WindowsDestinationPath.Replace("Namespace", "のはでした"))
                    )
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath.Replace("Namespace", "コンサート"),
                        WindowsDestinationPath.Replace("Namespace", "コンサート"))
                    )
                        yield return new object[] {source, destination};
                }

                foreach (var (source, destination) in AddPaths(UnixSourcePath, UnixDestinationPath))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(
                    UnixSourcePath.Replace("Namespace", "Пространствоимен"),
                    UnixDestinationPath.Replace("Namespace", "Пространствоимен")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(
                    UnixSourcePath.Replace("Namespace", "汉字漢字"),
                    UnixDestinationPath.Replace("Namespace", "汉字漢字")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(
                    UnixSourcePath.Replace("Namespace", "のはでした"),
                    UnixDestinationPath.Replace("Namespace", "のはでした")))
                    yield return new object[] {source, destination};
                foreach (var (source, destination) in AddPaths(
                    UnixSourcePath.Replace("Namespace", "コンサート"),
                    UnixDestinationPath.Replace("Namespace", "コンサート")))
                    yield return new object[] {source, destination};
            }

            private IEnumerable<(string, DocumentUri)> AddPaths(string source, DocumentUri destination)
            {
                yield return (source.Replace("c:", "c%3A"), destination);
                yield return (source.Replace("c:", "c%3a"), destination);
                yield return (source, destination);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class FileUriToFileSystem : IEnumerable<object[]>
        {
            private const string WindowsSourcePath = "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs";
            private const string UnixSourcePath = "/usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private const string WindowsDestinationPath =
                "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            private const string UnixDestinationPath = "file:///usr/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs";

            public IEnumerator<object[]> GetEnumerator()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath,
                        WindowsDestinationPath))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath.Replace("Namespace", "Пространствоимен"),
                        WindowsDestinationPath.Replace("Namespace", "Пространствоимен"))
                    )
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath.Replace("Namespace", "汉字漢字"),
                        WindowsDestinationPath.Replace("Namespace", "汉字漢字"))
                    )
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath.Replace("Namespace", "のはでした"),
                        WindowsDestinationPath.Replace("Namespace", "のはでした"))
                    )
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        WindowsSourcePath.Replace("Namespace", "コンサート"),
                        WindowsDestinationPath.Replace("Namespace", "コンサート"))
                    )
                        yield return new object[] {source, destination};
                }
                else
                {
                    foreach (var (source, destination) in AddPaths(UnixSourcePath, UnixDestinationPath))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        UnixSourcePath.Replace("Namespace", "Пространствоимен"),
                        UnixDestinationPath.Replace("Namespace", "Пространствоимен")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        UnixSourcePath.Replace("Namespace", "汉字漢字"),
                        UnixDestinationPath.Replace("Namespace", "汉字漢字")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        UnixSourcePath.Replace("Namespace", "のはでした"),
                        UnixDestinationPath.Replace("Namespace", "のはでした")))
                        yield return new object[] {source, destination};
                    foreach (var (source, destination) in AddPaths(
                        UnixSourcePath.Replace("Namespace", "コンサート"),
                        UnixDestinationPath.Replace("Namespace", "コンサート")))
                        yield return new object[] {source, destination};
                }
            }

            private IEnumerable<(Uri, DocumentUri)> AddPaths(string source, DocumentUri destination)
            {
                yield return (new Uri(source), destination);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}