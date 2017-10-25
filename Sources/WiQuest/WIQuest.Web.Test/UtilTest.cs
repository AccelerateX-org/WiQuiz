using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WIQuest.Web.Utils;

namespace WIQuest.Web.Test
{
    [TestFixture]
    public class UtilTest
    {
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
    }
}