namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Used by source generation to add generation options to a given assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AssemblyRegistrationOptionsAttribute : Attribute
    {
        public Type[] Types { get; }

        public AssemblyRegistrationOptionsAttribute(params Type[] registrationOptionsTypes)
        {
            Types = registrationOptionsTypes;
        }
    }
}
