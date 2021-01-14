using System;
using System.Collections.Generic;
using System.Linq;

namespace Anagramimation
{

public record AnimationGlobalConfig
{
    public bool EnableAnimation { get; init; } = true;

    public AnimationTiming Timing { get; init; } = AnimationTiming.linear;

    public int FontPixels { get; init; } = 30;

    public double FontPixelsDouble
    {
        get => FontPixels;
        init => FontPixels = Convert.ToInt32(value);
    }

    public double RelativeWidth { get; init; } = 1;

    public int GetWidth(WordList wordList) => Convert.ToInt32(FontPixels * RelativeWidth) * wordList.WordLengths.Max();

    public int GetHeight(IEnumerable<AnimationStepConfig> stepConfigs) => Convert.ToInt32(
        FontPixels * 2 * stepConfigs.Select(x => Math.Max(1, x.MaxHeightFactor)).Max()
    );
}

}
