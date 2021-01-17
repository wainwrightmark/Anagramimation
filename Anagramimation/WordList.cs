using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Anagramimation
{

public record WordList(
    ImmutableList<Path> Paths,
    ImmutableList<string> Words)
{
    private ImmutableList<int>? _wordLengths;

    public ImmutableList<int> WordLengths => _wordLengths ??=
        Words.Select(x => x.EnumerateRunes().Count()).ToImmutableList();

    public static readonly WordList Empty = new(
        ImmutableList<Path>.Empty,
        ImmutableList<string>.Empty
    );

    public static WordList Create(IEnumerable<string> words, CharMatchingConfig charMatching)
    {
        var result = words.Aggregate<string, WordList?>(
            null,
            (a, w) => a == null ? Create(w) : a.AddWord(w, charMatching)
        );

        if (result == null)
            return Empty;

        return result;
    }

    public static WordList Create(string wordString)
    {
        var word =
            wordString.EnumerateRunes()
                .Select(
                    (rune, index) => (
                        rune,
                        new Path(rune, ImmutableList.Create<Node?>(new Node(index, false, 0))))
                )
                .ToImmutableList();

        var paths = word.Select(x => x.Item2).ToImmutableList();

        return new WordList(paths, ImmutableList.Create(wordString));
    }

    [Pure]
    public WordList AddWord(string word, CharMatchingConfig config)
    {
        if (!Words.Any())
            return Create(word);

        var newRunes       = word.EnumerateRunes();
        var remainingRunes = newRunes.Select((rune, index) => (rune, index)).ToHashSet();
        var centre         = remainingRunes.Count / 2;

        var newPaths = new List<Path>();

        var pathsWithIndexes =
            Paths.Select(path => (path, lastIndex: path.Nodes.Last()?.RuneIndex));

        foreach (var (path, lastIndex) in pathsWithIndexes
            .OrderByDescending(x => x.lastIndex.HasValue)
            .ThenBy(
                x => x.lastIndex.HasValue ? Math.Abs(centre - x.lastIndex.Value) : null as int?
            ))
        {
            var possibleMatches =
                remainingRunes
                    .Select(other => (other, match: config.GetMatch(path.Rune, other.rune)))
                    .Where(x => x.match.Strength > 0)
                    .OrderByDescending(x => x.match.Strength)
                    .ThenByDescending(
                        x => lastIndex.HasValue ? Math.Abs(lastIndex.Value - x.other.index) : 0
                    )
                    .Take(1)
                    .ToList();

            Path newPath;

            if (possibleMatches.Any())
            {
                var (other, (_, reflect, degreesRotation)) = possibleMatches.Single();
                remainingRunes.Remove(other);

                newPath = path with
                {
                    Nodes = path.Nodes.Add(
                        new Node(other.index, reflect, degreesRotation)
                    )
                };
            }
            else
                newPath = path with { Nodes = path.Nodes.Add(null) };

            newPaths.Add(newPath);
        }

        var emptyNodes = Enumerable.Repeat(null as Node, Words.Count).ToImmutableList();

        foreach (var (rune, index) in remainingRunes)
        {
            var newNode = new Node(index, false, 0);
            var newPath = new Path(rune, emptyNodes.Add(newNode));

            newPaths.Add(newPath);
        }

        return new WordList(
            newPaths.ToImmutableList(),
            Words.Add(word)
        );
    }
}

}
