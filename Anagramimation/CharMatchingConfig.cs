using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anagramimation
{

public class CharMatchingConfig
{
    public CharMatch GetMatch(Rune a, Rune b)
    {
        if (a == b) return new CharMatch(100, false, 0);

        if (!CaseSensitive)
        {
            if (a.ToString().Equals(b.ToString(), StringComparison.OrdinalIgnoreCase))
                return new CharMatch(75, false, 0);
        }

        if ((AllowReflection || AllowRotation) && Dictionary.TryGetValue((a, b), out var match))
            if (AllowReflection || !match.Reflect)
                if (AllowRotation || match.DegreesRotation == 0)
                    return match;

        return CharMatch.NoMatch;

    }


    private static readonly IDictionary<(Rune, Rune), CharMatch> Dictionary =
        new List<(char a, char b, CharMatch match)>()
            {

                ( 'b', 'd', new CharMatch(50, true,  0)),
                ('b', 'p', new CharMatch(50, true,  180)),
                ('d', 'p', new CharMatch(50, false, 180)),
                ('m', 'w', new CharMatch(50, false, 180)),
                ('M', 'W', new CharMatch(50, false, 180)),
                ('u', 'n', new CharMatch(50, false, 180)),
                ('z', 'n', new CharMatch(50, false, 90)),
                ('u', 'c', new CharMatch(50, false, 90)),
            }
            .Select(x=> (a:new Rune(x.a), b:new Rune(x.b), x.match))
            .SelectMany(
                x =>
                {
                    var reverse = (a:x.b, b:x.a, match:x.match.Reverse);

                    return new [] { x, reverse };
                }).ToDictionary(x => (x.a, x.b), x => x.match);


    public bool CaseSensitive { get; set; }

    public bool AllowRotation { get; set; }

    public bool AllowReflection { get; set; }
}

}