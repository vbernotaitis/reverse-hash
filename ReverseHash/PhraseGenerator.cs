using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReverseHash
{
    public class PhraseGenerator
    {
        private string[] words;
        private Dictionary<char, int> charaters;
        private int phraseLenght;
        private int wordsCount;

        public int WordsCount => words.Length;

        public PhraseGenerator(string anagram, string[] words)
        {
            this.wordsCount = anagram.Count(x => x == ' ') + 1;
            this.phraseLenght = anagram.Length;
            this.charaters = ExtractCharacters(anagram);
            this.words = FilterValidWords(words);

            Console.WriteLine(this.words.Length);
        }

        private Dictionary<char, int> ExtractCharacters(string anagram)
        {
            anagram = anagram.Replace(" ", "");
            var characters = new Dictionary<char, int>();
            for (var i = 0; i < anagram.Length; i++)
            {
                var character = anagram[i];
                if (characters.ContainsKey(character))
                {
                    characters[character] = ++characters[character];
                }
                else
                {
                    characters.Add(character, 1);
                }
            }
            return characters;
        }

        private string[] FilterValidWords(string[] words)
        {
            var regPattern = $"^[{string.Join("", charaters.Keys)}]*$";
            var filteredWords = words
                .Where(x => x.Length < 17)
                .Where(x => Regex.IsMatch(x, regPattern))
                .Where(x => charaters.All(a => x.Count(b => b == a.Key) <= a.Value));
            var uniqueWords = new HashSet<string>(filteredWords);

            return uniqueWords.ToArray();
        }

        public IEnumerable<string> GetUniquePhrases(int startIndex, int endIndex)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                for (var j = 0; j < words.Length; j++)
                {
                    if (i == j || !AreAllLettersValid($"{words[i]} {words[j]}", 3, 17)) continue;
                    for (var k = 0; k < words.Length; k++)
                    {
                        var phrase = $"{words[i]} {words[j]} {words[k]}";
                        if (i == k || j == k || !AreAllLettersValid(phrase, phraseLenght, phraseLenght)) continue;
                        yield return phrase;
                    }
                }
            }
        }

        private bool AreAllLettersValid(string phrase, int minPhraseLenght, int maxPhraseLength)
        {
            var isPhraseLenghValid = phrase.Length >= minPhraseLenght && phrase.Length <= maxPhraseLength;
            var hasCorrectLettersCount = !charaters.Any(x => phrase.Count(a => a == x.Key) > x.Value);
            return isPhraseLenghValid && hasCorrectLettersCount;
        }
    }
}
