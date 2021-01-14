using Fluxor;

namespace Anagramimation
{

public static class Reducers
{
    [ReducerMethod]
    public static State Reduce(State state, IAction action) => action.Reduce(state);

}

}
