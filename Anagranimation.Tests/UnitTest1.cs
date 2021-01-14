using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Anagramimation.Tests
{

public class UnitTest1
{
    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
    }

    public ITestOutputHelper TestOutputHelper { get; set; }

    [Theory]
    [InlineData("elvis",  "lives",  5)]
    [InlineData("silent", "listen", 6)]
    public void TestWordList(string s1, string s2, int expectedPaths)
    {
        var wordList = WordList.Create(
            new[] { s1, s2 },
            new CharMatchingConfig()
            {
                AllowReflection = false, AllowRotation = false, CaseSensitive = true
            }
        );

        wordList.Paths.Count.Should().Be(expectedPaths);

        var newWord1 = string.Join(
            "",
            wordList.Paths.Where(x => x.Nodes[0] != null)
                .OrderBy(x => x.Nodes[0]!.RuneIndex)
                .Select(x => x.Letter)
                .ToList()
        );

        var newWord2 = string.Join(
            "",
            wordList.Paths.Where(x => x.Nodes[1] != null)
                .OrderBy(x => x.Nodes[1]!.RuneIndex)
                .Select(x => x.Letter)
                .ToList()
        );

        newWord1.Should().Be(s1);
        newWord2.Should().Be(s2);
    }

    [Fact]
    public void TestDirectPattern()
    {
        var global = new AnimationGlobalConfig() { FontPixels = 1, RelativeWidth = 1 };

        var step = new AnimationStepConfig()
        {
            MaxHeightFactor = 2,
            MinHeightFactor = 1,
            Clockwise       = true,
            Waypoint1       = 0.2,
            Waypoint2       = 0.4
        };

        var start = new Node(0, false, 0);
        var end   = new Node(1, false, 0);

        var startPercent = 0;
        var endPercent   = 100;

        var points = Pattern.Direct.Instance.GetStepPoints(
                global,
                step,
                2,
                start,
                end,
                startPercent,
                endPercent
            )
            .ToList();

        foreach (var animationPoint in points)
        {
            TestOutputHelper.WriteLine(animationPoint.ToString());
        }
    }

    [Theory]
    [InlineData("AB",   "BA")]
    [InlineData("MARK", "KRAM")]
    public void TestPath(string word1, string word2)
    {
        var state = new Feature().State;

        state = Reducers.Reduce(state, new SetWordAction(0, word1));
        state = Reducers.Reduce(state, new SetWordAction(1, word2));

        foreach (var wordListPath in state.WordList.Paths)
        {
            TestOutputHelper.WriteLine("");
            TestOutputHelper.WriteLine(wordListPath.Letter.ToString());
            TestOutputHelper.WriteLine("");

            foreach (var animationPoint in wordListPath.GetAnimationPoints(
                state.Config,
                state.StepConfigs,
                state.WordList.WordLengths
            ))
            {
                TestOutputHelper.WriteLine(animationPoint.ToString());
            }
        }
    }

    [Theory]
    [InlineData("AB", "BC", "CA")]
    public void Test3WordPath(string word1, string word2, string word3)
    {
        var words    = new List<string>() { word1, word2, word3 }.ToImmutableList();
        var wordlist = WordList.Create(words, new CharMatchingConfig());

        var state = new State(
            wordlist,
            new CharMatchingConfig(),
            new AnimationGlobalConfig(),
            new List<AnimationStepConfig>() { new(), new(), new() }.ToImmutableList()
        );

        foreach (var wordListPath in state.WordList.Paths)
        {
            TestOutputHelper.WriteLine("");
            TestOutputHelper.WriteLine(wordListPath.Letter.ToString());
            TestOutputHelper.WriteLine("");

            foreach (var animationPoint in wordListPath.GetAnimationPoints(
                state.Config,
                state.StepConfigs,
                state.WordList.WordLengths
            ))
            {
                TestOutputHelper.WriteLine(animationPoint.ToString());
            }
        }
    }

    [Fact]
    public void TestHtml()
    {
        var state = new Feature().State;

        var html = state.GetHtml();

        TestOutputHelper.WriteLine(html);
    }
}

}
