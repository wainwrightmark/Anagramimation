using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Anagramimation.Tests
{
    public class RecordTest
    {
        public record TestRecord(int A, int B);

        public IEnumerable<TestRecord> YieldRecordsOneAtATime()
        {
            var current = new TestRecord(1, 0);

            yield return current;
            current = current with { B = 1 };
            yield return current;

        }

        public IEnumerable<TestRecord> YieldRecordsAllAtOnce()
        {
            var current = new TestRecord(1, 0);


            var second = current with { B = 1 };
            yield return current;
            yield return second;

        }

        [Fact]
        public void TestRecordOneAtATime()
        {
            var records = YieldRecordsOneAtATime().ToList();

            records[0].B.Should().Be(0);
            records[1].B.Should().Be(1);
        }

    }
}
