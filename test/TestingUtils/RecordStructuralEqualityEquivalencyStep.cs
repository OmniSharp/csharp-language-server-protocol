using FluentAssertions.Equivalency;
using FluentAssertions.Equivalency.Steps;

namespace TestingUtils
{
    public class RecordStructuralEqualityEquivalencyStep : StructuralEqualityEquivalencyStep, IEquivalencyStep
    {
        EquivalencyResult IEquivalencyStep.Handle(Comparands comparands, IEquivalencyValidationContext context, IValidateChildNodeEquivalency nestedValidator)
        {
            return comparands.Subject?.GetType()?.GetMethod("<Clone>$") != null ? EquivalencyResult.EquivalencyProven : EquivalencyResult.ContinueWithNext;
        }
    }
}
