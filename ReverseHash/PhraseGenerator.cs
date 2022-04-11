using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReverseHash
{
    public class PhraseGenerator
    {
        private readonly string[] _availableWordsList;
        private readonly Dictionary<char, int> _availableCharaters;
        private readonly int _phraseLenght;
        private readonly int _wordsInPhrase;

        public int WordsInListCount => _availableWordsList.Length;

        public PhraseGenerator(string anagram, string[] wordsList)
        {
            _wordsInPhrase = anagram.Count(x => x == ' ') + 1;
            _phraseLenght = anagram.Length;
            _availableCharaters = ExtractCharacters(anagram);
            _availableWordsList = FilterInvalidWords(wordsList);

            Console.WriteLine(_availableWordsList.Length);
        }

        public IEnumerable<string> GetUniquePhrases(int startIndex, int endIndex) {
            var maxIndex = WordsInListCount - 1;
            foreach (var index in new IndexesEnumerable(maxIndex, new[] { startIndex, 0, 0 }, new[] { endIndex, maxIndex, maxIndex })) 
            {
                var phrase = GetPhraseByIndexes(index);
                if (!IsPhraseValid(phrase))
                    continue;
                yield return phrase;
            }
        }

        private string GetPhraseByIndexes(IEnumerable<int> wordsIndexes)
        {
            return string.Join(" ", wordsIndexes.Select(x => _availableWordsList[x]));
        }

        private bool HasCorrectLettersCount(string phrase) 
        {
            return _availableCharaters.All(a => phrase.Count(b => b == a.Key) <= a.Value);
        }

        private bool IsPhraseValid(string phrase)
        {
            var isPhraseLenghValid = phrase.Length == _phraseLenght;
            return isPhraseLenghValid && HasCorrectLettersCount(phrase);
        }

        private Dictionary<char, int> ExtractCharacters(string anagram)
        {
            anagram = anagram.Replace(" ", "");
            var characters = new Dictionary<char, int>();
            foreach (var character in anagram)
            {
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

        private string[] FilterInvalidWords(string[] words)
        {
            var regPattern = $"^[{string.Join("", _availableCharaters.Keys)}]*$";
            var maxWordLenght = _phraseLenght - _wordsInPhrase * 2;
            var filteredWords = words
                .Select(x => x.Trim().ToLower())
                .Where(x => x.Length <= maxWordLenght)
                .Where(x => Regex.IsMatch(x, regPattern))
                .Where(x => HasCorrectLettersCount(x));
            var uniqueWords = new HashSet<string>(filteredWords);

            return uniqueWords.ToArray();
        }
    }
}
