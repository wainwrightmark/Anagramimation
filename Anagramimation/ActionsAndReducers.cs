using System;

namespace Anagramimation
{

public interface IAction
{
    State Reduce(State state);
}

public record SetWordAction(int Index, string Word) : IAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        state = state with
        {
            WordList = WordList.Create(
                state.WordList.Words.SetItem(Index, Word),
                state.CharMatchingConfig
            )
        };

        return state;
    }
}

public record AddAction(string Word, AnimationStepConfig Config) : IAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        state = state with
        {
            WordList = state.WordList.AddWord(Word, state.CharMatchingConfig),
            StepConfigs = state.StepConfigs.Add(Config)
        };

        return state;
    }
}

public record SetGlobalConfigAction(Func<AnimationGlobalConfig, AnimationGlobalConfig> Func) : IAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        state = state with { Config = Func(state.Config) };
        return state;
    }
}

public record SetConfigAction(int Index, Func<AnimationStepConfig, AnimationStepConfig> Func) : IAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        state = state with
        {
            StepConfigs = state.StepConfigs.SetItem(Index, Func(state.StepConfigs[Index]))
        };

        return state;
    }
}

}
