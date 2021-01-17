using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Anagramimation
{

public static class Defaults
{
    private static readonly List<string> DefaultWords = new()
    {
        "Animate",
        "Anagram",
        "Go Create",
        "What you can",
        "Try some s p a c e",
        "Or a face 😀",
        "Someone's name?",
        "Make a chain",
        "That connects you",
        "To your flame 🔥",
        "Try the options",
        "There's a ton",
        "Get creative",
        "And have fun",
    };

    public static string SuggestWord()
    {
        var adjective = Adjectives.Value.GetRandom();
        var animal    = Animals.Value.GetRandom();

        return $"{adjective} {animal}";
    }

    public static string TryAnagramWord(string root)
    {
        var rootWords = root.ToLowerInvariant().Split(' ').ToHashSet();

            IEnumerable<string> Filter(IEnumerable<ImmutableList<string>> l)
            {
                return l
                    .Where(x=> !x.All(rootWords.Contains) )
                    .Select(anagram => string.Join(' ', anagram.Where(x => x.Length > 1)))
                    .ToList();
            }

            var wordListList = Filter(AnagramDictionary.Default.Value.GetAnagrams(root, 1))
                    .Take(10)
                    .ToList();

            if (!wordListList.Any())
            {
                wordListList =
                    Filter(AnagramDictionary.Default.Value.GetAnagrams(root, 3))
                        .Take(10)
                        .ToList();
            }

            if (wordListList.Any())
                return wordListList.GetRandom();

            return root;
    }

    private static readonly Lazy<IReadOnlyList<string>> Adjectives =
        new(
            () => Words.Adjectives.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            )
        );

    private static readonly Lazy<IReadOnlyList<string>> Animals =
        new(
            () => Words.Animals.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            )
        );

    public static string GetNextWord(int i, string previousWord)
    {
        if (i == 0)
            return DefaultWords[0];

        if (string.IsNullOrWhiteSpace(previousWord))
            return "";

        var previousWordIndex = DefaultWords.IndexOf(previousWord);

        if (previousWordIndex == i - 1 && i < DefaultWords.Count)
            return DefaultWords[i];

        return previousWord;
    }

    private static readonly List<string> PlaceHolders = new() { "Type Anything" };

    public static string GetPlaceHolder(int i) => PlaceHolders[i % PlaceHolders.Count];

    private static readonly IReadOnlyList<AnimationStepConfig> StepConfigs =
        new List<AnimationStepConfig> { new() };

    public static AnimationStepConfig GetStepConfig(int i) => StepConfigs[i % PlaceHolders.Count];
}

}
