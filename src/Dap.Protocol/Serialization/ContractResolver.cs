using System.Linq;
using System.Reflection;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Serialization
{
    class ContractResolver : DefaultContractResolver
    {
        public ContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy(true, false, true);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (member.GetCustomAttributes<OptionalAttribute>().Any()
             || property.DeclaringType.Name.EndsWith("Capabilities")
            )
            {
                property.NullValueHandling = NullValueHandling.Ignore;
            }

            return property;
        }
    }
}
