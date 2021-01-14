namespace Anagramimation
{

public record AnimationSummary(string Name, int DurationSeconds, AnimationTiming AnimationTiming)
{
    //TODO more fields
    /// <inheritdoc />
    public override string ToString() => $"{Name} {DurationSeconds}s {ToCssString(AnimationTiming)} infinite;";

    public static string ToCssString(AnimationTiming animationTiming) => animationTiming.ToString().Replace('_', '-');
}

}
