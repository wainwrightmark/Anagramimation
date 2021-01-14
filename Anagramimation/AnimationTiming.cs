namespace Anagramimation
{

public enum AnimationTiming
{
    /// <summary>
    /// Default value. The animation has a slow start, then fast, before it ends slowly
    /// </summary>
    ease = 1,

    /// <summary>
    /// The animation has the same speed from start to end
    /// </summary>
    linear = 2,

    /// <summary>
    /// The animation has a slow start
    /// </summary>
    ease_in = 3,

    /// <summary>
    /// The animation has a slow end
    /// </summary>
    ease_out = 4,

    /// <summary>
    /// The animation has both a slow start and a slow end
    /// </summary>
    ease_in_out = 5,

    /// <summary>
    /// Equivalent to steps(1, start)
    /// </summary>
    step_start = 6,

    /// <summary>
    /// Equivalent to steps(1, end)
    /// </summary>
    step_end = 7,

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
    initial = 8,

    /// <summary>
    /// Inherits this property from its parent element.
    /// </summary>
    inherit = 9
}

}
