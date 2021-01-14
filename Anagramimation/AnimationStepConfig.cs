using System;
using System.Runtime.CompilerServices;

namespace Anagramimation
{

public record AnimationStepConfig
{
    public Pattern Pattern { get; init; } = Pattern.Direct.Instance;


    public int DurationSeconds { get; init; } = 2;

    public double DurationSecondsDouble
    {
        get => DurationSeconds;
        init => DurationSeconds = Convert.ToInt32(value);
    }

    public bool Clockwise { get; init; }

    /// <summary>
    /// Rest duration
    /// </summary>
    public double Waypoint1 { get; init; } = 0.4;
    public double Waypoint2 { get; init; } = 0.7;
    public double Waypoint3 { get; init; } = 0.8;
    public double Waypoint4 { get; init; } = 0.9;
    public double Waypoint5 { get; init; } = 0.95;

    public const double DefaultMinHeight = 1;
    public const double DefaultMaxHeight = 2;

    public double MaxHeightFactor { get; init; } = 2;
    public double MinHeightFactor { get; init; } = 1;

    public double GetWaypoint(int i)
    {
        return i switch
        {
            1 => Waypoint1,
            2 => Waypoint2,
            3 => Waypoint3,
            4 => Waypoint4,
            5 => Waypoint5,
            _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
        };
    }

        public AnimationStepConfig SetWaypoint(int i, double v)
        {


            return i switch
            {
                1 => this with{Waypoint1 = v},
                2 => this with{Waypoint2 = v},
                3 => this with{Waypoint3 = v},
                4 => this with{Waypoint4 = v},
                5 => this with{Waypoint5 = v},
                _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
            };
        }
    }

}
