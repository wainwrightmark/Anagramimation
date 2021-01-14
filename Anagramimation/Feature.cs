using System.Collections.Immutable;
using Fluxor;

namespace Anagramimation
{

public class Feature : Feature<State>
{
    public override string GetName() => "State";

    protected override State GetInitialState() => new(
        WordList.Create(new[] { State.InitialValues[0].Word, State.InitialValues[1].Word, }, new CharMatchingConfig()),
        new CharMatchingConfig(),
        new AnimationGlobalConfig(),
        new[] { new AnimationStepConfig(), new AnimationStepConfig() }.ToImmutableList()
    );
}

}
