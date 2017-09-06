using System.Collections.Generic;
using System.Linq;
using Xunit;
using WIQuest.Web.Utils;

namespace WIQuest.Web.Test
{
    public class TestClass
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact (Skip = "Shall not fail!")]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        [Fact]
        public void AmIReallyShufflingEveryday()
        {
            // Prepare Mock
            var numbersToShuffle = new List<int>(Enumerable.Range(1, 75));
            var numbersToCompare = new List<int>(Enumerable.Range(1, 75));

            // Lists should be the same 1...75
            Assert.True(numbersToCompare.Intersect(numbersToShuffle).SequenceEqual(numbersToShuffle));
            
            // Using Method under Test
            numbersToShuffle.Shuffle();

            // List should now differ
            Assert.False(numbersToCompare.Intersect(numbersToShuffle).SequenceEqual(numbersToShuffle));
        }

        static int Add(int x, int y)
        {
            return x + y;
        }
    }
}