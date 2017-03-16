using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace Lsp.Tests
{
    class JsonFixtureAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            // new csproj
            // var root = @"..\..\..\";
            // old csproj
            var root = @"..\..\";
            var @namespace = typeof(JsonFixtureAttribute).Namespace;
            var folderPaths = (new[] { root })
                .Concat(
                    testMethod.DeclaringType.Namespace
                    .Split('.')
                    .Skip(@namespace.Split('.').Length)
                ).ToArray();
            var fixtureLocation = Path.Combine(folderPaths);
            Console.WriteLine(fixtureLocation);
            var fileName = $"{testMethod.DeclaringType.Name}_${testMethod.Name}.json";
            var filePath = Path.Combine(fixtureLocation, fileName);

            if (!File.Exists(filePath))
            {
                throw new XunitException($"Could find fixture for {testMethod.DeclaringType.Name}.${testMethod.Name}");
            }

            yield return new object[] { File.ReadAllText(filePath)?.Replace("\r\n", "\n") };//?.Replace("\n", "\r\n") };
        }
    }
}
