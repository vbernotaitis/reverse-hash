using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReverseHash;

namespace UnitTests
{
    [TestClass]
    public class SecretPhrasesFinderTests
    {
        [TestMethod]
        public void ShouldFindOneWordPhrase()
        {
            var wordsList = new[] {"some", "random", "word"};
            var hashes = new[] {"7ddf32e17a6ac5ce04a8ecbf782ca509"};
            var anagram = "domran";
            var phraseGenerator = new PhraseGenerator(anagram, wordsList);
            var finder = new SecretPhrasesFinder(phraseGenerator, hashes);
            var results = finder.FindSecretPhrases();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual($"random {hashes[0]}", results[0]);
        }

        [TestMethod]
        public void ShouldFindTwoWordPhrase()
        {
            var wordsList = new[] { "some", "random", "word" };
            var hashes = new[] { "90c234e95bce297cb5c382b9091c5a29" };
            var anagram = "medo somran";
            var phraseGenerator = new PhraseGenerator(anagram, wordsList);
            var finder = new SecretPhrasesFinder(phraseGenerator, hashes);
            var results = finder.FindSecretPhrases();

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual($"some random {hashes[0]}", results[0]);
        }

        [TestMethod]
        public void ShouldFindThreeWordPhrase()
        {
            var wordsList = new[] { "yups", "tortola", "untwist", "yuan", "tuna", "tutor", "tosup", "yawls", "stout", "printout"};
            var hashes = new[] { "e4820b45d2277f3844eac66c903e84be" };
            var anagram = "poultry outwits ants";
            var phraseGenerator = new PhraseGenerator(anagram, wordsList);
            var finder = new SecretPhrasesFinder(phraseGenerator, hashes);
            var results = finder.FindSecretPhrases();

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual($"printout stout yawls {hashes[0]}", results[0]);
        }
    }
}
