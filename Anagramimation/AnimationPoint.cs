using System.Text;

namespace Anagramimation
{

public record AnimationPoint(
    int Percentage,
    int? Top,
    int? Left,
    double? Opacity = 1,
    int? Rotation = 0,
    bool? Reflect = false) //TODO remove redundant data???
{
    public static readonly AnimationPoint Invisible = new (
        0,
        null,
        null,
        0,
        null,
        null
    );


    public string Style
    {
        get
        {
            StringBuilder data = new();

            if (Top.HasValue)
                data.Append($"top: {Top}px;");

            if (Left.HasValue)
                data.Append($"left: {Left}px;");

            if (Opacity != null)
                data.Append($"opacity: {Opacity:F2};");

            if (Reflect.HasValue || Rotation.HasValue)
            {
                StringBuilder transform = new();

                if (Reflect.HasValue)
                {
                    var degrees = Reflect.Value ? 180 : 0;
                    transform.Append($"rotateY({degrees}deg)");
                }

                if (Rotation.HasValue)
                    transform.Append($"rotate({Rotation.Value % 360}deg)");

                data.Append($"transform: {transform};");
            }

            return data.ToString();
        }
    }

    private string GetString()
    {
        var r = $"  {Percentage}% {{{Style}}}";
        return r;
    }

    private string? _keyFrame;

    public string KeyFrame => _keyFrame ??= GetString();

    /// <inheritdoc />
    public override string ToString() => KeyFrame;
}

}
