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
            _availableWordsList = FilterInvalidWords(wordsList, GetMaxSubPhraseLenght(1));

            Console.WriteLine(_availableWordsList.Length);
        }

        public IEnumerable<string> GetUniquePhrases(int startIndex, int endIndex)
        {
            var columnIndex = 0;
            var lastColumnIndex = _wordsInPhrase - 1;
            var wordsIndexes = new int[_wordsInPhrase];

            wordsIndexes[0] = startIndex;
            while (wordsIndexes[0] <= endIndex)
            {
                var areWordsInPhraseUnique = columnIndex == 0 || wordsIndexes[columnIndex - 1] != wordsIndexes[columnIndex];
                if (!areWordsInPhraseUnique)
                {
                    IncreaseWordsIndexes(wordsIndexes, columnIndex);
                    continue;
                }

                var wordsInCurrentPhrase = columnIndex + 1;
                var phrase = GetPhraseByIndexes(wordsIndexes.Take(wordsInCurrentPhrase));
                if (!IsPhraseValid(phrase, wordsInCurrentPhrase))
                {
                    IncreaseWordsIndexes(wordsIndexes, columnIndex);
                    continue;
                }

                if (columnIndex != lastColumnIndex)
                {
                    columnIndex++;
                    continue;
                }

                IncreaseWordsIndexes(wordsIndexes, columnIndex);
                yield return phrase;
            }
        }

        private bool IsPhraseValid(string phrase, int wordsInPhrase)
        {
            var minLenght = GetMinSubPhraseLenght(wordsInPhrase);
            var maxLength = GetMaxSubPhraseLenght(wordsInPhrase);
            return AreAllLettersValid(phrase, minLenght, maxLength);
        }

        private string GetPhraseByIndexes(IEnumerable<int> wordsIndexes)
        {
            return string.Join(" ", wordsIndexes.Select(x => _availableWordsList[x]));
        }

        private int GetMaxSubPhraseLenght(int wordsCount)
        {
            return _wordsInPhrase == wordsCount ? _phraseLenght : _phraseLenght - (_wordsInPhrase - wordsCount) * 2;
        }

        private int GetMinSubPhraseLenght(int wordsCount)
        {
            return _wordsInPhrase == wordsCount ? _phraseLenght : wordsCount * 2 - 1;
        }

        private bool AreAllLettersValid(string phrase, int minPhraseLenght, int maxPhraseLength)
        {
            var isPhraseLenghValid = phrase.Length >= minPhraseLenght && phrase.Length <= maxPhraseLength;
            var hasCorrectLettersCount = !_availableCharaters.Any(x => phrase.Count(a => a == x.Key) > x.Value);
            return isPhraseLenghValid && hasCorrectLettersCount;
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

        private string[] FilterInvalidWords(string[] words, int maxWordLenght)
        {
            var regPattern = $"^[{string.Join("", _availableCharaters.Keys)}]*$";
            var filteredWords = words
                .Where(x => x.Length <= maxWordLenght)
                .Where(x => Regex.IsMatch(x, regPattern))
                .Where(x => _availableCharaters.All(a => x.Count(b => b == a.Key) <= a.Value));
            var uniqueWords = new HashSet<string>(filteredWords);

            return uniqueWords.ToArray();
        }

        private void IncreaseWordsIndexes(int[] wordsIndexes, int columnIndex)
        {
            wordsIndexes[columnIndex]++;
            var maxWordsIndex = WordsInListCount - 1;
            for (var i = wordsIndexes.Length-1; i >= 0; i--)
            {
                var wordsIndex = wordsIndexes[i];
                var isNotFirstColumn = i - 1 >= 0;
                if (isNotFirstColumn && wordsIndex > maxWordsIndex)
                {
                    wordsIndexes[i] = 0;
                    wordsIndexes[i - 1]++;
                }
            }
        }
    }
}
