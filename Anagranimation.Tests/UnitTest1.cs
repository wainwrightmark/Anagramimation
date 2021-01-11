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
        [InlineData("elvis", "lives")]
        [InlineData("silent", "listen")]
        public void TestPairing(string s1, string s2)
        {
            var (newList1, newList2) = LetterPairer.PairUp(s1, s2,new CharMatchingConfig()
            {
                AllowReflection = false, AllowRotation = false,CaseSensitive = true
            } );

            newList1.Should().HaveCount(s1.Length);
            newList2.Should().HaveCount(s2.Length);

            newList1.Should().AllBeOfType<PairableLetter.Paired>().And.OnlyHaveUniqueItems(x=>(x as PairableLetter.Paired).OtherIndex);
            newList2.Should().AllBeOfType<PairableLetter.Paired>().And.OnlyHaveUniqueItems(x=>(x as PairableLetter.Paired).OtherIndex);;

            newList1.Select(x => x.Index).Should().BeInAscendingOrder();
            newList2.Select(x => x.Index).Should().BeInAscendingOrder();

            var newWord1 = string.Join("", newList1.Cast<PairableLetter.Paired>().Select(x => newList2[x.OtherIndex].Letter));
            var newWord2 = string.Join("", newList2.Cast<PairableLetter.Paired>().Select(x => newList1[x.OtherIndex].Letter));

            newWord1.Should().Be(s1);
            newWord2.Should().Be(s2);
        }

        [Theory]
        [InlineData("elvis", "lives", 5)]
        [InlineData("silent", "listen", 6)]
        public void TestPaths(string s1, string s2, int expectedPaths)
        {
            var (newList1, newList2) = LetterPairer.PairUp(s1, s2, new CharMatchingConfig()
            {
                AllowReflection = false,
                AllowRotation = false,
                CaseSensitive = true
            });

            var paths = LetterPairer.CreatePathArray(newList1, newList2);

            paths.Count.Should().Be(expectedPaths);

            var newWord1 = string.Join("",

            paths.Where(x => x.GetStart().HasValue)
                .OrderBy(x => x.GetStart())
                .Select(x => x.Letter)
                .ToList());

            var newWord2 = string.Join("",

            paths.Where(x => x.GetEnd().HasValue)
                .OrderBy(x => x.GetEnd())
                .Select(x => x.Letter)
                .ToList());

            newWord1.Should().Be(s1);
            newWord2.Should().Be(s2);

        }
    }
}
