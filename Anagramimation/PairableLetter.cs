namespace Anagramimation
{

public abstract record PairableLetter
{
    private PairableLetter(char letter, int index)
    {
        Letter = letter;
        Index  = index;
    }

    public char Letter { get; }
    public int Index { get; }

    public abstract Path ToPath(bool isFirst, int totalLength);

    public record UnPaired(char Letter, int Index, bool CouldPair) : PairableLetter(
        Letter,
        Index
    )
    {
        /// <inheritdoc />
        public override Path ToPath(bool isFirst, int totalLength) => isFirst
            ? new Path.Disappear(Letter, totalLength, Index)
            : new Path.Appear(Letter, totalLength, Index);
    }

    public record Paired(char Letter, int Index, int OtherIndex, bool Reflect, int DegreesRotation) :
        PairableLetter(
            Letter,
            Index
        )
    {
        public override Path ToPath(bool isFirst, int totalLength) => isFirst
            ? new Path.Move(Letter, totalLength, Index,      OtherIndex, Reflect, DegreesRotation)
            : new Path.Move(Letter, totalLength, OtherIndex, Index, Reflect, DegreesRotation);
    };
}

}
