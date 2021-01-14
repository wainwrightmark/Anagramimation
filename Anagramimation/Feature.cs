using System.Collections.Immutable;
using System.Text;
using Fluxor;

namespace Anagramimation
{
    public class Feature : Feature<State>
    {
        public override string GetName() => "State";

        protected override State GetInitialState() => new (
            WordList.Create(new[] { "Animate", "Anagram", }, new CharMatchingConfig()),
            new CharMatchingConfig(),
            new AnimationGlobalConfig(),
            new[] { new AnimationStepConfig(), new AnimationStepConfig() }.ToImmutableList()
        );


    }


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

    //public class State
    //{
    //    public State(WordList wordList, CharMatchingConfig charMatchingConfig, AnimationGlobalConfig config, ImmutableList<AnimationStepConfig> stepConfigs)
    //    {
    //        WordList = wordList;
    //        CharMatchingConfig = charMatchingConfig;
    //        Config = config;
    //        StepConfigs = stepConfigs;
    //    }
    //    public WordList WordList { get; set; }
    //    public CharMatchingConfig CharMatchingConfig { get; set; }
    //    public AnimationGlobalConfig Config { get; set; }
    //    public ImmutableList<AnimationStepConfig> StepConfigs { get; set; }
    //}

}
