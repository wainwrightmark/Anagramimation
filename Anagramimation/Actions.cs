﻿using System;
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
    int Index { get; }
}

public record SetWordAction(int Index, string Word) : IWordChangedAction
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
        dispatcher.Dispatch(new JumpToAction(action.Index));
        await Task.Delay(100);
        dispatcher.Dispatch(new RestoreAnimateAction());
    }
}

public record PauseAnimateAction : IAction
{
    /// <inheritdoc />b
    public State Reduce(State state) =>
        state with { Config = state.Config with { EnableAnimation = false } };
}

public record RestoreAnimateAction : IAction
{
    public State Reduce(State state)
    {
        return state with { Config = state.Config with { EnableAnimation = true } };
    }
}

public record JumpToAction(int AnimationDelayIndex) : IAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        var totalDelay = state.StepConfigs
            .Take(AnimationDelayIndex)
            .Select(x => x.DurationSeconds)
            .DefaultIfEmpty(0)
            .Sum();

        return state with { Config = state.Config with { AnimationDelaySeconds = totalDelay } };
    }
}

public record RemoveWordAction(int Index) : IWordChangedAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        var newState = state with
        {
            WordList = WordList.Create(
                state.WordList.Words.RemoveAt(Index),
                state.CharMatchingConfig
            ),
            StepConfigs = state.StepConfigs.RemoveAt(Index)
        };

        return newState;
    }
}

public record SuggestWordAction(int Index) : IWordChangedAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        var newWord = Defaults.SuggestWord();

        state = state with
        {
            WordList = WordList.Create(
                state.WordList.Words.SetItem(Index, newWord),
                state.CharMatchingConfig
            )
        };

        return state;
    }
}

public record AnagramWordAction(int Index) : IWordChangedAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        string currentWord;

        if (Index <= state.WordList.Words.Count)
            currentWord = state.WordList.Words[Index];
        else
            currentWord = "";

        var newWord = Defaults.TryAnagramWord(currentWord);

        state = state with
        {
            WordList = WordList.Create(
                state.WordList.Words.SetItem(Index, newWord),
                state.CharMatchingConfig
            )
        };

        return state;
    }
}

public record AddAction(int Index) : IWordChangedAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        var previousWord =
            Index >= 0 &&
            Index <= state.WordList.Words.Count
                ? state.WordList.Words[Index - 1]
                : "";

        var newWord  = Defaults.GetNextWord(Index, previousWord);
        var newWords = state.WordList.Words.Insert(Index, newWord);
        var config   = Defaults.GetStepConfig(Index);

        state = state with
        {
            WordList = WordList.Create(newWords, state.CharMatchingConfig),
            StepConfigs = state.StepConfigs.Insert(Index, config)
        };

        return state;
    }
}

public record SetGlobalConfigAction(
    Func<AnimationGlobalConfig, AnimationGlobalConfig> Func) : IAction
{
    /// <inheritdoc />
    public State Reduce(State state)
    {
        state = state with { Config = Func(state.Config) };
        return state;
    }
}

public record SetConfigAction(
    int Index,
    Func<AnimationStepConfig, AnimationStepConfig> Func) : IAction
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
