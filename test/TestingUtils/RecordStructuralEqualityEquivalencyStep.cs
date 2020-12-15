using FluentAssertions.Equivalency;

namespace TestingUtils
{
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
