using FluentAssertions.Equivalency;
using FluentAssertions.Equivalency.Steps;

namespace TestingUtils
{
    public class RecordStructuralEqualityEquivalencyStep : StructuralEqualityEquivalencyStep, IEquivalencyStep
    {
        EquivalencyResult IEquivalencyStep.Handle(Comparands comparands, IEquivalencyValidationContext context, IEquivalencyValidator nestedValidator)
        {
            return comparands.Subject?.GetType()?.GetMethod("<Clone>$") != null ? EquivalencyResult.AssertionCompleted : EquivalencyResult.ContinueWithNext;
        }
    }
}
