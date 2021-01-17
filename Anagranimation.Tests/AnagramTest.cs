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
            Dictionary  = new AnagramDictionary(Words.AllWords);
        }

        public AnagramDictionary Dictionary;

        [Theory]
        [InlineData("tab", "bat")]
        [InlineData("silent", "listen")]
        [InlineData("Clint Eastwood", "act dew soliton")]
        [InlineData("Mark Wainwright", "hawk trigram win")]
        public void TestAnagrams(string text, string expected)
        {
            var sw = Stopwatch.StartNew();
            var anagrams = Dictionary.GetAnagrams(text)
                .Select(x=>x.OrderBy(w=>w))
                .Select(a=>string.Join(' ',a))
                .Take(1000).ToList();

            TestOutputHelper.WriteLine(sw.Elapsed.ToString());
            foreach (var anagram in anagrams)
            {
                TestOutputHelper.WriteLine(anagram);
            }

            anagrams.Should().Contain(expected);



        }
    }
}
