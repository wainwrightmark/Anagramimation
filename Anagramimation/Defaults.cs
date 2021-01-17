using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        var rootWords = root.ToLowerInvariant().Split(' ').ToHashSet(StringComparer.OrdinalIgnoreCase);

        Console.WriteLine($"Finding Anagrams for: {root}");

        var sw = Stopwatch.StartNew();
        IEnumerable<string> Filter(IEnumerable<ImmutableList<string>> l)
        {
            return l
                .Where(x => !x.All(rootWords.Contains))
                .Select(anagram => string.Join(' ', anagram))
                .ToList();
        }

        var dictionary = AnagramDictionary.Default.Value;
        Console.WriteLine($"Created Dictionary {dictionary.Dictionary.Count} words. {sw.Elapsed.TotalSeconds}s");

        var wordListList = Filter(dictionary.GetAnagrams(root, 1))
            .Take(10)
            .ToList();

        Console.WriteLine($"Found {wordListList.Count} single word anagrams. {sw.Elapsed.TotalSeconds}s");

        if (!wordListList.Any())
        {
            wordListList =
                Filter(dictionary.GetAnagrams(root, 2))
                    .Take(10)
                    .ToList();

            Console.WriteLine($"Found {wordListList.Count}  anagrams. {sw.Elapsed.TotalSeconds}s");
        }

        if (wordListList.Any())
        {
            Console.WriteLine("Returning random.");
            return wordListList.GetRandom();
        }

        return root;
    }

    private static readonly Lazy<IReadOnlyList<string>> Adjectives =
        new(
            () => WordsResource.Adjectives.Split(
                new[] { '\r', '\n' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            )
        );

    private static readonly Lazy<IReadOnlyList<string>> Animals =
        new(
            () => WordsResource.Animals.Split(
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
