namespace Anagramimation.Blazor.Flux
{

public record OpenEditingConfig(int Index) : IAction
{
    /// <inheritdoc />
    public State Reduce(State vs) => vs with { EditingConfigIndex = Index };
}

public record CloseEditingConfig : IAction
{
    /// <inheritdoc />
    public State Reduce(State vs) => vs with { EditingConfigIndex = null };
}

}
