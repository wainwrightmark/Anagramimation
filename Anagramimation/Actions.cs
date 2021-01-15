using System;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;

namespace Anagramimation
{

public interface IAction
{
    State Reduce(State state);
}

public interface IWordChangedAction : IAction
    {
        int Index{get;}
    }

public record SetWordAction(int Index, string Word) :  IWordChangedAction
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

    public class SetWordEffect : Effect<IWordChangedAction>
    {
        /// <inheritdoc />
        protected override async Task HandleAsync(IWordChangedAction action, IDispatcher dispatcher)
        {
            dispatcher.Dispatch(new PauseAnimateAction());
            await ValueTask.CompletedTask;
            await Task.Delay(100);
            dispatcher.Dispatch(new RestoreAnimateAction(action.Index));
        }
    }


    public record PauseAnimateAction : IAction
    {
        /// <inheritdoc />b 
        public State Reduce(State state) => state with { Config = state.Config with { EnableAnimation = false} };
    }

    public record RestoreAnimateAction(int AnimationDelayIndex) : IAction
    {
        public State Reduce(State state)
        {
            var totalDelay = state.StepConfigs
                .Take(AnimationDelayIndex)
                .Select(x => x.DurationSeconds)
                .DefaultIfEmpty(0)
                .Sum();

            return state with { Config = state.Config with { EnableAnimation = true, AnimationDelaySeconds = totalDelay} };
        }
    }


    public record RemoveWordAction(int Index) : IWordChangedAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
            state = state with
            {
                WordList = WordList.Create(
                    state.WordList.Words.RemoveAt(Index),
                    state.CharMatchingConfig
                ),
                StepConfigs = state.StepConfigs.RemoveAt(Index)
            };

            return state;
        }
}

public record AddAction(string Word, int Index, AnimationStepConfig Config) : IWordChangedAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {

        var newWords = state.WordList.Words.Insert(Index, Word);

        state = state with
        {
            WordList = WordList.Create(newWords, state.CharMatchingConfig),
            StepConfigs = state.StepConfigs.Insert(Index, Config)
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
