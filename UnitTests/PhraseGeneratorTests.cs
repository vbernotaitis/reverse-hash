using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReverseHash;

namespace UnitTests
{
    [TestClass]
    public class PhraseGeneratorTests
    {
        [TestMethod]
        public void ShouldReturnAllCombinations()
        {
            var anagram = "some random word";
            var wordsList = new[] {"some", "random", "word"};
            var allCombinationsCount = 6;
            var phraseGenerator = new PhraseGenerator(anagram, wordsList);

            var allPhrases = phraseGenerator.GetUniquePhrases(0, wordsList.Length - 1).ToArray();
            foreach (var phrase in allPhrases)
            {
                Console.WriteLine(phrase);
            }

            Assert.AreEqual(allCombinationsCount, allPhrases.Length);
        }

        [TestMethod]
        public void ShouldReturnJustValidPhrases()
        {
            var anagram = "some random word";
            var wordsList = new[] {"some", "random", "word", "none", "ran"};
            var allCombinationsCount = 6;
            var phraseGenerator = new PhraseGenerator(anagram, wordsList);

            var allPhrases = phraseGenerator.GetUniquePhrases(0, phraseGenerator.WordsInListCount - 1).ToArray();
            foreach (var phrase in allPhrases)
            {
                Console.WriteLine(phrase);
            }

            Assert.AreEqual(allCombinationsCount, allPhrases.Length);
        }

        [TestMethod]
        public void ShouldReturnAllPhrasesWhenAllWordsSameLenght()
        {
            var anagram = "abcd acbd adbc";
            var wordsList = new[]{ "abcd", "acbd", "adbc", "bcad", "badc", "cadb", "cbda", "dabc", "dbac", "dcab"};
            var phraseGenerator = new PhraseGenerator(anagram, wordsList);
            var allCombinationsCount = 10 * 9 * 8;

            var allPhrases = phraseGenerator.GetUniquePhrases(0, phraseGenerator.WordsInListCount - 1).ToArray();
            foreach (var phrase in allPhrases)
            {
                Console.WriteLine(phrase);
            }

            Assert.AreEqual(allCombinationsCount, allPhrases.Length);
        }
    }
}
