using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Anagramimation
{

public record State(
    WordList WordList,
    CharMatchingConfig CharMatchingConfig,
    AnimationGlobalConfig Config,
    ImmutableList<AnimationStepConfig> StepConfigs)
{

    public static readonly IReadOnlyList<(string Word, string PlaceHolder)> InitialValues =
        new List<(string Word, string PlaceHolder)>
        {
            ("Animate", "Try Your Name"),
            ("Anagram", "Try a Similar Word"),
            ("Maniac", "Be creative!"),
            ("A n i m a t e", "Try using spaces"),
            ("🤖 Animate 🤖", "Try Emoji"),
            ("Animate", "Have fun!"),
        };


    public string GetHtml()
    {
        var sb = new StringBuilder();

        sb.AppendLine("<div class=\"area\">");

        foreach (var path in WordList.Paths)
        {
            var style = path.GetStyle(Config, StepConfigs);

            sb.AppendLine($"<span class=\"letter\" style=\"{style}\">{path.Letter}</span>");
        }

        sb.AppendLine("</div>");

        //TODO improve

        sb.AppendLine("<style>");
        sb.AppendLine("\t.letter { position: absolute; font-family: monospace; }");
        sb.AppendLine($".area {{font-size: {Config.FontPixels}px;position: relative;}}");
        sb.AppendLine($".word {{position: absolute}}");

        foreach (var path in WordList.Paths)
        {
            sb.AppendLine(path.GetAnimationDefinition(Config, StepConfigs, WordList.WordLengths));
        }

        sb.AppendLine("</style>");

        return sb.ToString();
    }
}

}
