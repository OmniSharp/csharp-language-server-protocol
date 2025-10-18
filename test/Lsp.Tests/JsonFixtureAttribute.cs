using System.Reflection;
using Xunit.Sdk;

namespace Lsp.Tests
{
    internal class JsonFixtureAttribute : DataAttribute
    {
        public Assembly Resources = typeof(JsonFixtureAttribute).GetTypeInfo().Assembly;

        public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
        {
            var fileName = $"{testMethod.DeclaringType!.FullName}_${testMethod.Name}.json";

            if (!Resources.GetManifestResourceNames().Contains(fileName))
            {
                throw new XunitException($"Could find fixture for {testMethod.DeclaringType.Name}.${testMethod.Name}");
            }

            using (var streamReader = new StreamReader(Resources.GetManifestResourceStream(fileName)!))
            {
                yield return new object?[] { streamReader.ReadToEnd().Replace("\r\n", "\n").TrimEnd() };
            }
        }
    }
}
