using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReverseHash;

namespace UnitTests
{
    [TestClass]
    public class HashCheckerTests
    {
        [TestMethod]
        public void ShouldReturnMatchingHash()
        {
            const string phrase = "some random words";
            const string hash = "7df370135bc249bfef9fe70832e2cc3f";
            var hashChecker = new HashChecker();

            var actualValue = hashChecker.GetMatchingHash(phrase, new[] { hash });

            Assert.AreEqual(hash, actualValue);
        }
    }
}
