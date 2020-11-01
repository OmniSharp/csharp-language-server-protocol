using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistrationOptionsConverter
    {
        Type SourceType { get; }
        Type DestinationType { get; }
        string Key { get; }
        object? Convert(object source);
    }

    public interface IRegistrationOptionsConverter<in TSource, out TDestination> : IRegistrationOptionsConverter
        where TSource : IRegistrationOptions
        where TDestination : class?
    {
        TDestination Convert(TSource source);
    }

    public abstract class RegistrationOptionsConverterBase<TSource, TDestination> : IRegistrationOptionsConverter<TSource, TDestination>
        where TSource : IRegistrationOptions
        where TDestination : class
    {
        public RegistrationOptionsConverterBase(string key)
        {
            Key = key;
        }

        public Type SourceType { get; } = typeof(TSource);
        public Type DestinationType { get; }= typeof(TDestination);
        public string Key { get; }
        public object? Convert(object source) => source is TSource value ? Convert(value) : null;
        public abstract TDestination Convert(TSource source);
    }
}
