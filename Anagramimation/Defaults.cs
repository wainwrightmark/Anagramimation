using System.Collections.Generic;

namespace Anagramimation
{

public static class Defaults
{
    private static readonly IReadOnlyList<string> DefaultWords = new List<string>()
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

    public static string GetWord(int i) => DefaultWords[i % DefaultWords.Count];

    private static readonly IReadOnlyList<string> PlaceHolders = new List<string>()
    {
        "Type Something"
    };

    public static string GetPlaceHolder(int i) => PlaceHolders[i % PlaceHolders.Count];

    private static readonly IReadOnlyList<AnimationStepConfig> StepConfigs = new List<AnimationStepConfig>
    {
        new ()
    };
    public static  AnimationStepConfig GetStepConfig(int i) =>StepConfigs[i % PlaceHolders.Count];
}

}
