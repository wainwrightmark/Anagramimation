using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Anagramimation
{

    public record WordList(
            ImmutableList<Path> Paths,
            ImmutableList<ImmutableList<(Rune rune, Path path)>> RunePaths,
            ImmutableList<string> Words)
    {
        private ImmutableList<int>? _wordLengths;

        public ImmutableList<int> WordLengths => _wordLengths??= RunePaths.Select(x=>x.Count).ToImmutableList();


        public static WordList Create( IEnumerable<string> words, CharMatchingConfig charMatching)
        {
            return words.Aggregate<string, WordList?>(
                null,
                (a, w) => a == null ? Create(w) : a.AddWord(w, charMatching)
            )!;
        }

        public static WordList Create(string wordString)
        {
            var word =
                wordString.EnumerateRunes().Select(
                        (rune, index) => (
                            rune, new Path(rune, ImmutableList.Create<Node?>(new Node(index, false, 0))))
                    )
                    .ToImmutableList();

            var paths = word.Select(x => x.Item2).ToImmutableList();

            return new WordList(paths, ImmutableList.Create( word), ImmutableList.Create(wordString));
        }

        [Pure]
        public  WordList AddWord(string word, CharMatchingConfig config)
        {
            var newRunes       = word.EnumerateRunes();
            var mostRecentWord = RunePaths.Last();
            var l1Centre       = mostRecentWord.Count / 2;

            var remainingLetters = newRunes.Select((rune, index) => (rune, index)).ToHashSet();


            //todo do better
            var remainingPaths = Paths.ToHashSet();

            var newPathWord = new List<(Rune, Path, int index)>();
            var newPaths = new List<Path>();

            foreach (var (mrwIndex, _) in Enumerable.Range(0, mostRecentWord.Count).Select(index => (index, distance: Math.Abs(l1Centre - index))).OrderBy(x => x.distance))
            {
                var (rune, path) = mostRecentWord[mrwIndex];
                remainingPaths.Remove(path);

                var possibleMatches =
                    remainingLetters
                        .Select(other => (other, match: config.GetMatch(rune, other.rune)))
                        .Where(x => x.match.Strength > 0)
                        .OrderByDescending(x => x.match.Strength)
                        .ThenByDescending(x => Math.Abs(mrwIndex - x.other.index))
                        .Take(1)
                        .ToList();

                Path newPath;

                if (possibleMatches.Any())
                {
                    var (other, match) = possibleMatches.Single();
                    remainingLetters.Remove(other);
                    newPath = path with { Nodes = path.Nodes.Add(new Node(other.index, match.Reflect, match.DegreesRotation)) };
                    newPathWord.Add((rune, newPath, other.index));
                }
                else
                {
                    newPath = path with { Nodes = path.Nodes.Add(null) };
                }
                newPaths.Add(newPath);

            }

            var emptyNodes = Enumerable.Repeat(null as Node, RunePaths.Count).ToImmutableList();

            foreach (var (rune, index) in remainingLetters) //TODO pair up with remaining paths
            {
                var newNode = new Node(index, false, 0);
                var path = new Path(rune, emptyNodes.Add(newNode));
                newPaths.Add(path);
                newPathWord.Add((rune, path, index));
            }

            newPaths.AddRange(remainingPaths.Select(rp=> rp with{Nodes = rp.Nodes.Add(null)}));


            return new WordList(newPaths.ToImmutableList(),
                                RunePaths.Add(newPathWord.Select(x => (x.Item1, x.Item2)).ToImmutableList()),
                                Words.Add(word));
        }
    }

}
