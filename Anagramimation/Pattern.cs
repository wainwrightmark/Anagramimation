using System;
using System.Collections.Generic;
using System.Linq;

namespace Anagramimation
{

public enum PatternEnum
{
    Direct,
    HalfFactory,

    //FullFactory,
    Explosion
}

public abstract record Pattern(string Name, int NumberOfWaypoints, PatternEnum PatternEnum)
{
    public static IEnumerable<Pattern> GetAll()
    {
        yield return Direct.Instance;
        yield return HalfFactory.Instance;
        yield return Explosion.Instance;
    }

    public static Pattern GetPattern(PatternEnum pe) => GetAll().Single(x => x.PatternEnum == pe);


    public abstract string GetWaypointName(int i);

    private static string DefaultWaypointName(int i) => $"Waypoint {i}";


    public record Direct() : Pattern(nameof(Direct), 2, PatternEnum.Direct)
    {
        public static Direct Instance { get; } = new();

        /// <inheritdoc />
        public override string GetWaypointName(int i)
        {
            return i switch
            {
                1 => "Rest",
                2 => "Top",
                _ => DefaultWaypointName(i)
            };
        }

        /// <inheritdoc />
        public override IEnumerable<AnimationPoint> GetStepPoints(
            AnimationGlobalConfig globalConfig,
            AnimationStepConfig stepConfig,
            int totalLength,
            Node start,
            Node end,
            int startPercent,
            int endPercent)
        {
            var (height, _, _) = GetPositionData(
                globalConfig,
                stepConfig,
                totalLength,
                start,
                end
            );

            var startPoint = start.GetBasePoint(startPercent, globalConfig);

            var afterRestPoint = startPoint with
            {
                Percentage = GetPercentage(startPercent, endPercent, stepConfig.Waypoint1),
            };

            var topPoint = afterRestPoint with
            {
                Percentage = GetPercentage(startPercent, endPercent, stepConfig.Waypoint2),
                Top = height,
                Left = null,
                Opacity = null,
                Rotation = null,
                Reflect = null
            };

            yield return startPoint;     //Start
            yield return afterRestPoint; //After rest
            yield return topPoint;       //Top of parabola
        }
    }

    public record HalfFactory() : Pattern(nameof(HalfFactory), 3, PatternEnum.HalfFactory)
    {
        public static HalfFactory Instance { get; } = new();

        /// <inheritdoc />
        public override IEnumerable<AnimationPoint> GetStepPoints(
            AnimationGlobalConfig globalConfig,
            AnimationStepConfig stepConfig,
            int totalLength,
            Node start,
            Node end,
            int startPercent,
            int endPercent)
        {
            var (height, startLeft, endLeft) = GetPositionData(
                globalConfig,
                stepConfig,
                totalLength,
                start,
                end
            );

            var startPoint = new AnimationPoint(
                startPercent,
                0,
                startLeft,
                1,
                start.RotationDegrees,
                start.Reflect
            );

            var afterRestPoint = startPoint with
            {
                Percentage = GetPercentage(startPercent, endPercent, stepConfig.Waypoint1),
            };

            var aboveStart = afterRestPoint with
            {
                Percentage = GetPercentage(startPercent, endPercent, stepConfig.Waypoint2),
                Top = height,
                Left = startLeft,
                Opacity = null,
                Rotation = null,
                Reflect = null
            };

            var aboveEnd = aboveStart with
            {
                Percentage = GetPercentage(startPercent, endPercent, stepConfig.Waypoint3),
                Top = height,
                Left = endLeft,
                Opacity = null,
                Rotation = null,
                Reflect = null
            };

            yield return startPoint;     //Start
            yield return afterRestPoint; //After Rest
            yield return aboveStart;     //above start
            yield return aboveEnd;       //Above end
        }

            /// <inheritdoc />
            public override string GetWaypointName(int i)
            {
                return i switch
                {
                    1 => "Rest",
                    2 => "Above Start",
                    3 => "Above End",
                    _ => DefaultWaypointName(i)
            };
            }
        }

    public record Explosion() : Pattern(nameof(Explosion), 3, PatternEnum.Explosion)
    {
        public static Explosion Instance { get; } = new();

            /// <inheritdoc />
            public override string GetWaypointName(int i)
            {
                return i switch
                {
                    1 => "Rest",
                    2 => "Compress",
                    3 => "Explode",
                    _ => DefaultWaypointName(i)
            };
            }

            /// <inheritdoc />
            public override IEnumerable<AnimationPoint> GetStepPoints(
            AnimationGlobalConfig globalConfig,
            AnimationStepConfig stepConfig,
            int totalLength,
            Node start,
            Node end,
            int startPercent,
            int endPercent)
        {
            var (height, startLeft, endLeft) = GetPositionData(
                globalConfig,
                stepConfig,
                totalLength,
                start,
                end
            );

            var startPoint = new AnimationPoint(
                startPercent,
                0,
                startLeft,
                1,
                start.RotationDegrees,
                start.Reflect
            );

            var afterRestPoint = startPoint with
            {
                Percentage = GetPercentage(startPercent, endPercent, stepConfig.Waypoint1),
            };

            var centerPoint = afterRestPoint with
            {
                Percentage = GetPercentage(startPercent, endPercent, stepConfig.Waypoint2),
                Top = 0,
                Left = Convert.ToInt32(
                    totalLength * globalConfig.FontPixels * globalConfig.RelativeWidth / 2
                ),
                Opacity = null,
                Rotation = null,
                Reflect = null
            };

            var aboveEndPoint = centerPoint with
            {
                Percentage = GetPercentage(startPercent, endPercent, stepConfig.Waypoint3),
                Top = height,
                Left = endLeft,
                Opacity = null,
                Rotation = null,
                Reflect = null
            };

            yield return startPoint;     //Start
            yield return afterRestPoint; //After rest
            yield return centerPoint;    //Center

            yield return aboveEndPoint; //Above End
        }
    }

    public abstract IEnumerable<AnimationPoint> GetStepPoints(
        AnimationGlobalConfig globalConfig,
        AnimationStepConfig stepConfig,
        int totalLength,
        Node start,
        Node end,
        int startPercent,
        int endPercent);

    protected static (int height, int startLeft, int endLeft) GetPositionData(
        AnimationGlobalConfig globalConfig,
        AnimationStepConfig stepConfig,
        int totalLength,
        Node start,
        Node end)
    {
        var heightSign = start.RuneIndex < end.RuneIndex == stepConfig.Clockwise ? 1 : -1;
        var length     = Math.Abs(start.RuneIndex - end.RuneIndex);

        var maxHeight = globalConfig.FontPixels * stepConfig.MaxHeightFactor;
        var minHeight = globalConfig.FontPixels * stepConfig.MinHeightFactor;

        var height = ((maxHeight + minHeight) * length * heightSign) / (totalLength * 2);

        var startLeft = start.RuneIndex * globalConfig.FontPixels * globalConfig.RelativeWidth;
        var endLeft   = end.RuneIndex * globalConfig.FontPixels * globalConfig.RelativeWidth;

        return (Convert.ToInt32(height), Convert.ToInt32(startLeft), Convert.ToInt32(endLeft));
    }

    public static int GetPercentage(
        int startPercent,
        int endPercent,
        double proportion)
    {
        var r = Convert.ToInt32((endPercent - startPercent) * proportion) + startPercent;

        return r;
    }
}

}
