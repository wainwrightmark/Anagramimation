using System;
using System.Collections.Immutable;
using System.Linq;

namespace Anagramimation
{

public class LetterPairer
{

    public static (ImmutableArray<PairableLetter> newList1, ImmutableArray<PairableLetter> newList2) PairUp(string s1, string s2, CharMatchingConfig config)
    {
        var l1 = ToPairableLetters(s1);
        var l2 = ToPairableLetters(s2);

        return PairUp(l1, l2, config);
    }


    public static ImmutableList<Path> CreatePathArray(
        ImmutableArray<PairableLetter> l1,
        ImmutableArray<PairableLetter> l2)
    {
        var totalLength = l1.Concat(l2)
            .Select(
                x =>
                {
                    return
                        x switch
                        {
                            PairableLetter.Paired => 1,
                            PairableLetter.UnPaired => 2,
                            _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
                        };
                }
            )
            .DefaultIfEmpty(0)
            .Sum() / 2;

        var paths =
            l1.Select(x => (x, true))
                .Concat(l2.Select(x => (x, false)))
                .Select(x => x.x.ToPath(x.Item2, totalLength))
                .GroupBy(x=>(x.GetStart(), x.GetEnd()))
                .Select(x=>x.First())
                .ToImmutableList();

        return paths;
    }




    public static ImmutableArray<PairableLetter> ToPairableLetters(string s) => s.Select((x, i) => new PairableLetter.UnPaired(x, i, x != ' '))
        .ToImmutableArray<PairableLetter>();

    public static (ImmutableArray<PairableLetter> newList1, ImmutableArray<PairableLetter> newList2)
        PairUp(ImmutableArray<PairableLetter> list1, ImmutableArray<PairableLetter> list2, CharMatchingConfig config)
    {
        var l1Centre = list1.Length / 2;
        var l2Centre = list2.Length / 2;

        var indexes1 = Enumerable.Range(0, list1.Length)
            .Select(index => (index, distance: Math.Abs(l1Centre - index), l1: true));

        var indexes2 = Enumerable.Range(0, list2.Length)
            .Select(index => (index, distance: Math.Abs(l2Centre - index), l1: false));

        var allIndexes = indexes1.Concat(indexes2)
            .OrderBy(x => x.distance)
            .Select(x => (x.index, x.l1));

        var current1 = list1;
        var current2 = list2;

        foreach (var (index, l1) in allIndexes)
        {
            var thisList  = l1 ? current1 : current2;
            var otherList = l1 ? current2 : current1;


            var tl = thisList[index];

            if (tl is PairableLetter.UnPaired { CouldPair: true })
            {
                var possibleMatches =
                    otherList.Where(
                            x =>
                                x is PairableLetter.UnPaired { CouldPair: true }
                        )
                        .Select(other=> (other, match: config.GetMatch(tl.Letter, other.Letter)))
                        .Where(x=>x.match.Strength > 0)
                        .OrderByDescending(x=>x.match.Strength)
                        .ThenByDescending(x => Math.Abs(index - x.other.Index))
                        .Take(1)
                        .ToList();

                if (possibleMatches.Any())
                {
                    var m = possibleMatches.Single();

                    var newThis      = new PairableLetter.Paired(tl.Letter, index,   m.other.Index, m.match.Reflect, m.match.DegreesRotation);
                    var newOther     = new PairableLetter.Paired(m.other.Letter,  m.other.Index, index, m.match.Reflect,(360 - m.match.DegreesRotation) % 360 );
                    var newThisList  = thisList.SetItem(index, newThis);
                    var newOtherList = otherList.SetItem(m.other.Index, newOther);

                    current1 = l1? newThisList : newOtherList;
                    current2 = l1? newOtherList : newThisList;
                }
            }
        }

        return (current1, current2);
    }
}

}
