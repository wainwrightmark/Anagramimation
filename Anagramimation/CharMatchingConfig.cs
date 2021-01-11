using System.Collections.Generic;
using System.Linq;

namespace Anagramimation
{

public class CharMatchingConfig
{
    public CharMatch GetMatch(char a, char b)
    {
        if (a == b) return new CharMatch(100, false, 0);

        if (!CaseSensitive)
            if (char.ToLowerInvariant(a) == char.ToLowerInvariant(b))
                return new CharMatch(75, false, 0);


        if ((AllowReflection || AllowRotation) && Dictionary.TryGetValue((a, b), out var match))
            if (AllowReflection || !match.Reflect)
                if (AllowRotation || match.DegreesRotation == 0)
                    return match;

        return CharMatch.NoMatch;

    }


    private static readonly IDictionary<(char, char), CharMatch> Dictionary =
        new List<(char a, char b, CharMatch match)>()
            {

                ('b', 'd', new CharMatch(50, true,  0)),
                ('b', 'p', new CharMatch(50, true,  180)),
                ('d', 'p', new CharMatch(50, false, 180)),
                ('m', 'w', new CharMatch(50, false, 180)),
                ('M', 'W', new CharMatch(50, false, 180)),
                ('u', 'n', new CharMatch(50, false, 180)),
            }
            .SelectMany(
                x =>
                {
                    var reverse = (x.b, x.a, x.match.Reverse);

                    return new (char a, char b, CharMatch match)[] { x, reverse };
                }).ToDictionary(x => (x.a, x.b), x => x.match);


    public bool CaseSensitive { get; set; }

    public bool AllowRotation { get; set; }

    public bool AllowReflection { get; set; }
}

}