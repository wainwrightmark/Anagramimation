using System;
using System.Collections.Generic;
using System.Text;

namespace Anagramimation
{

public class AnimationConfig
{
    public int RestPercent { get; set; }
    public int SpacingPixels { get; set; }
    public int DurationSeconds { get; set; }
    public int HeightFactor { get; set; }
    public AnimationTiming Timing { get; set; }

    public PathFinding PathFinding { get; set; }
}

public enum PathFinding
{
    Direct,
    Parabola,
    Factory,
    SuperFactory
}

public abstract record Path
{
    private Path(char letter, int totalLength)
    {
        Letter      = letter;
        TotalLength = totalLength;
    }

    public char Letter { get; }
    public int TotalLength { get; }

    public record Move(char Letter, int TotalLength, int Start, int End, bool Reflect, int Rotate) : Path(Letter, TotalLength)
    {
        /// <inheritdoc />
        public override string GetStyle(AnimationConfig config, bool enableAnimation)
        {
            var summary = GetAnimationSummary(config);
            var sb      = new StringBuilder();
            sb.AppendLine($"left:{config.SpacingPixels * Start}px;");

            if (enableAnimation)
                sb.AppendLine($"animation:{summary}");

            return sb.ToString();
        }

        /// <inheritdoc />
        public override string AnimationName => $"Move{Start}to{End}";

        /// <inheritdoc />
        public override IEnumerable<AnimationPoint> GetAnimationPoints(AnimationConfig config)
        {
            var isLeft = Start < End;
            var length = Math.Abs(Start - End);

            var maxHeight = config.SpacingPixels * config.HeightFactor;

            var topTop = config.PathFinding switch
            {
                PathFinding.Direct       => 0,
                PathFinding.Parabola => maxHeight * (isLeft ? -1 : 1) * length / TotalLength,
                PathFinding.Factory => maxHeight * (isLeft ? -1 : 1) * length / TotalLength,
                PathFinding.SuperFactory => maxHeight * (isLeft ? -1 : 1),
                _                        => throw new ArgumentOutOfRangeException()
            };

            var startLeft = Start * config.SpacingPixels;
            var endLeft   = End * config.SpacingPixels;
            var midLeft       = (startLeft + endLeft) / 2;

            const int midPercent = 50;
            var       rest2      = 100 - config.RestPercent;

            var mid1 = (midPercent + config.RestPercent) / 2;
            var mid2 = midPercent;
            var mid3 = (midPercent + rest2) / 2;

            switch (config.PathFinding) //TODO factory going all the way round
            //TODO explosion
            {
                case PathFinding.Parabola:
                case PathFinding.Direct:
                {
                    yield return new AnimationPoint(0,                  0,      startLeft, null);
                    yield return new AnimationPoint(config.RestPercent, 0,      startLeft, null);
                    yield return new AnimationPoint(mid2,                 topTop, midLeft,       null);
                    yield return new AnimationPoint(rest2,              0,      endLeft,   null, Rotate, Reflect);
                    yield return new AnimationPoint(100,                0,      endLeft,   null, Rotate, Reflect);

                    break;
                }
                case PathFinding.Factory:
                case PathFinding.SuperFactory:
                {
                    yield return new AnimationPoint(0, 0, startLeft, null);
                    yield return new AnimationPoint(config.RestPercent, 0, startLeft, null);
                    yield return new AnimationPoint(mid1, topTop, startLeft, null);
                    yield return new AnimationPoint(mid2, topTop, midLeft, null);
                    yield return new AnimationPoint(mid3, topTop, endLeft, null);
                    yield return new AnimationPoint(rest2, 0, endLeft, null, Rotate, Reflect);
                    yield return new AnimationPoint(100, 0, endLeft, null, Rotate, Reflect);

                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    public record Disappear(char Letter, int TotalLength, int Start) : Path(Letter, TotalLength)
    {
        /// <inheritdoc />
        public override string GetStyle(AnimationConfig config, bool enableAnimation)
        {
            var summary = GetAnimationSummary(config);
            var sb      = new StringBuilder();
            sb.AppendLine($"left:{config.SpacingPixels * Start}px;");
            sb.AppendLine($"opacity:1;");

            if (enableAnimation)
                sb.AppendLine($"animation:{summary}");

            return sb.ToString();
        }

        /// <inheritdoc />
        public override string AnimationName => $"DisappearFrom{Start}";

        /// <inheritdoc />
        public override IEnumerable<AnimationPoint> GetAnimationPoints(AnimationConfig config)
        {
            yield return new AnimationPoint(0,                        null, null, 1);
            yield return new AnimationPoint(config.RestPercent,       null, null, 1);
            yield return new AnimationPoint(50,                       null, null, 0.5);
            yield return new AnimationPoint(100 - config.RestPercent, null, null, 0);
            yield return new AnimationPoint(100,                      null, null, 0);
        }
    }

    public record Appear(char Letter, int TotalLength, int End) : Path(Letter, TotalLength)
    {
        /// <inheritdoc />
        public override string GetStyle(AnimationConfig config, bool enableAnimation)
        {
            var summary = GetAnimationSummary(config);
            var sb      = new StringBuilder();
            sb.AppendLine($"left:{config.SpacingPixels * End}px;");
            sb.AppendLine($"opacity:0;");

            if (enableAnimation)
                sb.AppendLine($"animation:{summary}");

            return sb.ToString();
        }

        /// <inheritdoc />
        public override string AnimationName => $"AppearAt{End}";

        /// <inheritdoc />
        public override IEnumerable<AnimationPoint> GetAnimationPoints(AnimationConfig config)
        {
            yield return new AnimationPoint(0,                        null, null, 0);
            yield return new AnimationPoint(config.RestPercent,       null, null, 0);
            yield return new AnimationPoint(50,                       null, null, 0.5);
            yield return new AnimationPoint(100 - config.RestPercent, null, null, 1);
            yield return new AnimationPoint(100,                      null, null, 1);
        }
    }

    public int? GetStart()
    {
        return this switch
        {
            Disappear appear => appear.Start,
            Appear           => null,
            Move move        => move.End,
            _                => throw new ArgumentOutOfRangeException()
        };
    }

    public int? GetEnd()
    {
        return this switch
        {
            Disappear        => null,
            Appear disappear => disappear.End,
            Move move        => move.End,
            _                => throw new ArgumentOutOfRangeException()
        };
    }

    public string GetAnimationDefinition(AnimationConfig config)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"@keyframes {AnimationName}{{");

        foreach (var point in GetAnimationPoints(config))
        {
            sb.AppendLine(point.ToString());
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public abstract string GetStyle(AnimationConfig config, bool enableAnimation);
    public abstract string AnimationName { get; }

    public AnimationSummary GetAnimationSummary(AnimationConfig config) => new(
        AnimationName,
        config.DurationSeconds,
        config.Timing
    );

    public abstract IEnumerable<AnimationPoint> GetAnimationPoints(AnimationConfig config);
}

}

public record AnimationSummary(string Name, int DurationSeconds, AnimationTiming AnimationTiming)
{
    //TODO more fields
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} {DurationSeconds}s {AnimationTiming.ToCssString()}  alternate infinite;";
    }
}

public record AnimationPoint(int Percentage, int? Top, int? Left, double? Opacity, int Rotation = 0, bool Reflect = false)
{
    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder data = new();

        if (Top.HasValue)
            data.Append($"top: {Top}px;");

        if (Left.HasValue)
            data.Append($"left: {Left}px;");

        if (Opacity.HasValue)
            data.Append($"opacity: {Opacity.Value:F2};");

        var rotation = Rotation % 360;

        if (rotation > 0)
            data.Append($"transform: rotate({rotation}deg);");

        if (Reflect)
            data.Append("transform: rotateY(180deg);");

        return $"  {Percentage}% {{{data}}}";

    }
}

public static class AnimationHelper
{
    public static string ToCssString(this AnimationTiming animationTiming)
    {
        return animationTiming.ToString().Replace('_', '-');
    }
}

public enum AnimationTiming
{
    /// <summary>
    /// Default value. The animation has a slow start, then fast, before it ends slowly
    /// </summary>
    ease,

    /// <summary>
    /// The animation has the same speed from start to end
    /// </summary>
    linear,

    /// <summary>
    /// The animation has a slow start
    /// </summary>
    ease_in,

    /// <summary>
    /// The animation has a slow end
    /// </summary>
    ease_out,

    /// <summary>
    /// The animation has both a slow start and a slow end
    /// </summary>
    ease_in_out,

    /// <summary>
    /// Equivalent to steps(1, start)
    /// </summary>
    step_start,

    /// <summary>
    /// Equivalent to steps(1, end)
    /// </summary>
    step_end,

    ///// <summary>
    ///// Specifies a stepping function, with two parameters.
    ///// The first parameter specifies the number of intervals in the function.
    ///// It must be a positive integer (greater than 0).
    ///// The second parameter, which is optional, is either the value "start" or "end",
    ///// and specifies the point at which the change of values occur within the interval.
    ///// If the second parameter is omitted, it is given the value "end"
    ///// </summary>
    //steps, TODO
    ///// <summary>
    ///// Define your own values in the cubic-bezier function
    ///// Possible values are numeric values from 0 to 1
    ///// </summary>
    //cubic_bezier,
    /// <summary>
    /// Sets this property to its default value.
    /// </summary>
    initial,

    /// <summary>
    /// Inherits this property from its parent element.
    /// </summary>
    inherit
}
