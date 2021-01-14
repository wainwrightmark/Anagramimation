using System.Collections.Immutable;
using System.Text;

namespace Anagramimation
{



//public abstract record PairableLetter
//{
//    private PairableLetter(Rune letter, int index)
//    {
//        Letter = letter;
//        Index  = index;
//    }

//    public Rune Letter { get; }
//    public int Index { get; }

//    public abstract Path ToPath(bool isFirst, int totalLength);

//    public record UnPaired(Rune Letter, int Index, bool CouldPair) : PairableLetter(
//        Letter,
//        Index
//    )
//    {
//        /// <inheritdoc />
//        public override Path ToPath(bool isFirst, int totalLength) => isFirst
//            ? new Path.Disappear(Letter, totalLength, Index)
//            : new Path.Appear(Letter, totalLength, Index);
//    }

//    public record Paired(Rune Letter, int Index, int OtherIndex, bool Reflect, int DegreesRotation) :
//        PairableLetter(
//            Letter,
//            Index
//        )
//    {
//        public override Path ToPath(bool isFirst, int totalLength) => isFirst
//            ? new Path.Move(Letter, totalLength, Index,      OtherIndex, Reflect, DegreesRotation)
//            : new Path.Move(Letter, totalLength, OtherIndex, Index, Reflect, DegreesRotation);
//    };
//}

}
