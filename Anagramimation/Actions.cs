using System;
using System.Threading.Tasks;
using Fluxor;

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

//public class SetWordEffect : Effect<SetWordAction>
//{
//    /// <inheritdoc />
//    protected override async Task HandleAsync(SetWordAction action, IDispatcher dispatcher) => dispatcher.Dispatch(new PauseAnimateAction());
//}


//public record PauseAnimateAction : IAction
//    {
//        /// <inheritdoc />
//        public State Reduce(State state) => state with { Config = state.Config with { EnableAnimation = false } };
//    }

//public record RestoreAnimateAction : IAction
//{
//    public State Reduce(State state) => state with { Config = state.Config with { EnableAnimation = true } };
//}

//public class PauseAnimateEffect : Effect<PauseAnimateAction>
//    {
//        /// <inheritdoc />
//        protected override async Task HandleAsync(PauseAnimateAction action, IDispatcher dispatcher)
//        {
//            await Task.Delay(2000);
//            dispatcher.Dispatch(new RestoreAnimateAction());
//        }
//    }


public record RemoveWordAction(int Index) : IAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
            state = state with
            {
                WordList = WordList.Create(
                    state.WordList.Words.RemoveAt(Index),
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
