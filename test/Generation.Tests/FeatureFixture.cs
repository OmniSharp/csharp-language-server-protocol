using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Generation.Tests
{
    internal static class FeatureFixture
    {
        public static Assembly Resources = typeof(FeatureFixture).GetTypeInfo().Assembly;

        public static string ReadSource(string featurePath)
        {
            var fileName = $"{Resources.GetName().Name}.Features.{featurePath}";

            if (!Resources.GetManifestResourceNames().Contains(fileName))
            {
                throw new XunitException($"Could find fixture for {fileName}");
            }

            using var streamReader = new StreamReader(Resources.GetManifestResourceStream(fileName)!);
            return streamReader.ReadToEnd().Replace("\r\n", "\n").Replace("namespace OmniSharp.Extensions.LanguageServer.Protocol.Bogus", "namespace Test").TrimEnd();
        }
    }
}
