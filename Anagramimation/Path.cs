using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Anagramimation
{

public record Node(int RuneIndex, bool Reflect, int RotationDegrees)
{
    public AnimationPoint GetBasePoint(int percent, AnimationGlobalConfig globalConfig)
    {
        var left =
            Convert.ToInt32(globalConfig.FontPixels * globalConfig.RelativeWidth * RuneIndex);

        return new AnimationPoint(percent, 0, left, 1, RotationDegrees, Reflect);
    }
};

public record Path(Rune Rune, ImmutableList<Node?> Nodes)
{
    public string GetAnimationDefinition(
        AnimationGlobalConfig config,
        IReadOnlyList<AnimationStepConfig> stepConfigs,
        ImmutableList<int> wordLengths)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"@keyframes {AnimationName}{{");

        foreach (var point in GetAnimationPoints(config, stepConfigs, wordLengths))
            sb.AppendLine(point.ToString());

        sb.AppendLine("}");
        return sb.ToString();
    }

    public string AnimationName
    {
        get
        {
            var (node, index) = Nodes.Select((x, i) => (x, i)).First(x => x.x != null);

            if (Rune.IsAscii && char.IsLetter((char)Rune.Value))
                return $"KF_{Rune}_{node!.RuneIndex}_{index}";
            else
                return $"KF_{node!.RuneIndex}_{index}";
        }
    }

    public string GetStyle(
        AnimationGlobalConfig globalConfig,
        IReadOnlyList<AnimationStepConfig> stepConfigs)
    {
        var left = Convert.ToInt32(
            globalConfig.FontPixels * globalConfig.RelativeWidth *
            Nodes.First(x => x != null)!.RuneIndex
        );

        AnimationPoint animationPoint;
        var            first = Nodes.First();

        if (first == null)
            animationPoint = AnimationPoint.Invisible with { Left = left };
        else
            animationPoint = first.GetBasePoint(0, globalConfig);

        var style = animationPoint.Style;

        if (globalConfig.EnableAnimation)
        {
            var summary = GetAnimationSummary(globalConfig, stepConfigs);
            style += $"\r\nanimation:{summary}";
            if(globalConfig.AnimationDelaySeconds > 0)
                style += $"animation-delay: -{globalConfig.AnimationDelaySeconds}s;";
        }

        return style;
    }

    public AnimationSummary GetAnimationSummary(
        AnimationGlobalConfig config,
        IReadOnlyList<AnimationStepConfig> stepConfigs) => new(
        AnimationName,
        stepConfigs.Sum(x => x.DurationSeconds),
        config.Timing
    );

    public IEnumerable<AnimationPoint> GetAnimationPoints(
        AnimationGlobalConfig globalConfig,
        IReadOnlyList<AnimationStepConfig> stepConfigs,
        IReadOnlyList<int> wordLengths)
    {
        var totalDuration = stepConfigs.Sum(x => x.DurationSeconds);

        var left = Convert.ToInt32(
            globalConfig.FontPixels * globalConfig.RelativeWidth *
            Nodes.First(x => x != null)!.RuneIndex
        );

        Debug.Assert(
            stepConfigs.Count == Nodes.Count,
            "Should be same number of stepConfigs and nodes"
        );

        Debug.Assert(
            stepConfigs.Count == wordLengths.Count,
            "Should be same number of stepConfigs and wordLengths"
        );

        var currentPercentage = 0;

        for (var i = 0; i < Nodes.Count; i++)
        {
            var node       = Nodes[i];
            var stepConfig = stepConfigs[i];

            var nextPercentage =
                currentPercentage + (stepConfig.DurationSeconds * 100 / totalDuration);
            var restPercentage = Pattern.GetPercentage(currentPercentage, nextPercentage, stepConfig.Waypoint1);
            if (node == null)
            {
                yield return AnimationPoint.Invisible with { Percentage = currentPercentage, Left = left};//TODO factor in rest points

                yield return AnimationPoint.Invisible with{ Percentage = restPercentage, Left = left};

            }
            else
            {
                var nextNodeIndex = i + 1 >= Nodes.Count ? 0 : i + 1;
                var nextNode      = Nodes[nextNodeIndex];

                if (nextNode == null)
                {
                    var startLeft = Convert.ToInt32(
                        node.RuneIndex * globalConfig.FontPixels * globalConfig.RelativeWidth
                    );

                        var disappearNode = new AnimationPoint(
                            currentPercentage,
                            0,
                            startLeft,
                            1,
                            node.RotationDegrees,
                            node.Reflect
                        );

                        var restNode = disappearNode with { Percentage = restPercentage };

                        yield return disappearNode;
                        yield return restNode;
                }
                else
                {
                    var maxWordLength = Math.Max(wordLengths[i], wordLengths[nextNodeIndex]);

                    foreach (var ap in stepConfig.Pattern.GetStepPoints(
                        globalConfig,
                        stepConfig,
                        maxWordLength,
                        node,
                        nextNode,
                        currentPercentage,
                        nextPercentage
                    ))
                        yield return ap;
                }
            }

            currentPercentage = nextPercentage;
        }

        var first = Nodes.First();

        if (first == null)
            yield return AnimationPoint.Invisible with { Percentage = 100, Left = left};
        else
        {
            yield return first.GetBasePoint(100, globalConfig);
        }
    }
}

}
