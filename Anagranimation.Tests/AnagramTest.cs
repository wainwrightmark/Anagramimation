using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Anagramimation.Tests
{
    public class AnagramTest
    {
        public ITestOutputHelper TestOutputHelper { get; }

        public AnagramTest(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
            Dictionary = new AnagramDictionary(Words.AllWords);
        }

        public AnagramDictionary Dictionary;

        [Theory]
        [InlineData("tab", 2, "bat")]
        [InlineData("silent", 2, "listen")]
        [InlineData("Clint Eastwood", 2, "coil downstate")]
        [InlineData("Clint Eastwood", 3, "old west action")]
        [InlineData("Mark Wainwright", 3, "hawk trigram win")]
        [InlineData("Mark Wainwright", 2, "hawk trigram win")]
        [InlineData("Mark Wainwright", 4, "hawk trigram win")]
        [InlineData("tome", 2, "me to")]
        public void TestAnagrams(string text, int words, string expected)
        {
            var sw = Stopwatch.StartNew();
            var anagrams = Dictionary.GetAnagrams(text, words)
                .Select(x => x.OrderBy(w => w))
                .Select(a => string.Join(' ', a))
                .Take(10000).ToList();

            TestOutputHelper.WriteLine(anagrams.Count + " anagrams found");
            TestOutputHelper.WriteLine(sw.Elapsed.ToString());
            foreach (var anagram in anagrams)
            {
                TestOutputHelper.WriteLine(anagram);
            }

            anagrams.Should().Contain(expected);



        }
    }
}
