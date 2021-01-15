using System.Collections.Immutable;
using Fluxor;

namespace Anagramimation
{

public class Feature : Feature<State>
{
    public override string GetName() => "State";

    protected override State GetInitialState() => new(
        WordList.Create(new[] { Defaults.GetWord(0) , Defaults.GetWord(1) }, new CharMatchingConfig()),
        new CharMatchingConfig(),
        new AnimationGlobalConfig(),
        new[] { new AnimationStepConfig(), new AnimationStepConfig() }.ToImmutableList()
    );
}

}
