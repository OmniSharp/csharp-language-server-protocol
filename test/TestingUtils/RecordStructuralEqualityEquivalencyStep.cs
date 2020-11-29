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

    public class RecordStructuralEqualityEquivalencyStep : StructuralEqualityEquivalencyStep, IEquivalencyStep
    {
        bool IEquivalencyStep.CanHandle(
            IEquivalencyValidationContext context,
            IEquivalencyAssertionOptions config
        )
        {
            return context?.Subject?.GetType()?.GetMethod("<Clone>$") != null;
        }
    }
}
