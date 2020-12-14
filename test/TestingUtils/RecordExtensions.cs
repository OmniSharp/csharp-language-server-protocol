using FluentAssertions.Equivalency;

namespace TestingUtils
{
    public static class RecordExtensions
    {
        public static EquivalencyAssertionOptions<TExpectation> UsingStructuralRecordEquality<TExpectation>(this EquivalencyAssertionOptions<TExpectation> options)
        {
            return options.Using(new RecordStructuralEqualityEquivalencyStep());
        }
    }
}