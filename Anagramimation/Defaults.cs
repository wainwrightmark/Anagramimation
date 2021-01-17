using System;
using System.Collections.Generic;
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

    public static string GetNextWord(int i, string previousWord)
    {
        if (i == 0)
            return DefaultWords[0];

        if (string.IsNullOrWhiteSpace(previousWord))
            return "";

        var previousWordIndex = DefaultWords.IndexOf(previousWord);

        if (previousWordIndex == i - 1 && i < DefaultWords.Count)
            return DefaultWords[i];

        var anagrams =
            AnagramDictionary.Default.Value.GetAnagrams(previousWord).Take(10)
                .Select(anagram=> string.Join(' ', anagram.Where(x => x.Length > 1)))
                .Where(x=> !x.Equals(previousWord, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (!anagrams.Any()) // Slightly hacky  - replace with a random animal
            anagrams = AnagramDictionary.Animals.Value.Dictionary.Values
                .Select(anagram=> string.Join(' ', anagram.Where(x => x.Length > 1)))
                .Where(x=> !x.Equals(previousWord, StringComparison.OrdinalIgnoreCase))
                .ToList();


        if (!anagrams.First().Contains(' ')) //prioritize full anagrams
            return anagrams.First();

        var r = new Random();
        var chosen = anagrams[r.Next(anagrams.Count)];


        return chosen;
    }

    private static readonly List<string> PlaceHolders = new()
    {
        "Type Anything"
    };

    public static string GetPlaceHolder(int i) => PlaceHolders[i % PlaceHolders.Count];

    private static readonly IReadOnlyList<AnimationStepConfig> StepConfigs = new List<AnimationStepConfig>
    {
        new ()
    };
    public static  AnimationStepConfig GetStepConfig(int i) =>StepConfigs[i % PlaceHolders.Count];
}

}
