using FluentAssertions.Equivalency;

namespace TestingUtils
{
    public static class RecordExtensions
    {
        public static EquivalencyOptions<TExpectation> UsingStructuralRecordEquality<TExpectation>(this EquivalencyOptions<TExpectation> options)
        {
            return options.Using(new RecordStructuralEqualityEquivalencyStep());
        }
    }
}
