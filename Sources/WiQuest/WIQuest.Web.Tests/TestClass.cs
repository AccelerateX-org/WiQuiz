using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WIQuest.Web.Utils;

namespace WIQuest.Web.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        [Ignore("FailingTest")]
        public void FailingTest()
        {
            Assert.AreEqual(5, Add(2, 2));
        }

        [Test]
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