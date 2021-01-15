namespace Anagramimation
{

public record CharMatch(int Strength, bool Reflect, int DegreesRotation)
{
    public static readonly CharMatch NoMatch = new(0, false, 0);

    public CharMatch Reverse => this with { DegreesRotation = (360 - DegreesRotation) % 360 };

    /// <inheritdoc />
    public override string ToString()
    {
        return (Strength, Reflect, DegreesRotation).ToString();
    }
}

}