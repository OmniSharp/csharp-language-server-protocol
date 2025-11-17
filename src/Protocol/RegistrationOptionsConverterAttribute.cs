namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegistrationOptionsConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        public RegistrationOptionsConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
