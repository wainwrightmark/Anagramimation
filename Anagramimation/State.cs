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
    public string GetHtml()
    {
        var sb = new StringBuilder();

        sb.AppendLine("<div class=\"area\">");

        foreach (var path in WordList.Paths)
        {
            var style = path.GetStyle(Config, StepConfigs);

            sb.AppendLine($"<span class=\"Rune\" style=\"{style}\">{path.Rune}</span>");
        }

        sb.AppendLine("</div>");

        //TODO improve

        sb.AppendLine("<style>");

        sb.AppendLine("@import url('https://fonts.googleapis.com/css2?family=Maven+Pro:wght@500&display=swap');");

        sb.AppendLine("\t.Rune { position: absolute; font-family: 'Maven Pro', sans-serif; }");
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
