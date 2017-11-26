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
        private int wordsInPhrase;

        public int WordsInListCount => words.Length;

        public PhraseGenerator(string anagram, string[] words)
        {
            this.wordsInPhrase = anagram.Count(x => x == ' ') + 1;
            this.phraseLenght = anagram.Length;
            this.charaters = ExtractCharacters(anagram);
            this.words = FilterValidWords(words, GetMaxSubPhraseLenght(1));

            Console.WriteLine(this.words.Length);
        }

        public IEnumerable<string> GetUniquePhrases(int startIndex, int endIndex)
        {
            var columnIndex = 0;
            var lastColumnIndex = wordsInPhrase - 1;
            var indexes = new int[wordsInPhrase];
            indexes[0] = startIndex;
            while (indexes[0] < endIndex)
            {
                var phrase = GetPhrase(indexes.Take(columnIndex + 1).ToArray());
                var minLenght = GetMinSubPhraseLenght(columnIndex + 1);
                var maxLength = GetMaxSubPhraseLenght(columnIndex + 1);
                var areNotEqual = columnIndex == 0 || indexes[columnIndex - 1] != indexes[columnIndex];
                if (areNotEqual && AreAllLettersValid(phrase, minLenght, maxLength))
                {
                    columnIndex++;
                }
                else
                {
                    IncreaseIndexes(indexes, columnIndex);
                }

                if (columnIndex == lastColumnIndex)
                {
                    for (var i = 0; i < WordsInListCount; i++)
                    {
                        indexes[lastColumnIndex] = i;
                        var fullphrase = GetPhrase(indexes);
                        if (AreAllLettersValid(fullphrase, phraseLenght, phraseLenght))
                        {
                            yield return fullphrase;
                        }
                    }
                    IncreaseIndexes(indexes, columnIndex);
                    columnIndex = 0;
                }
            }
        }

        private int GetMaxSubPhraseLenght(int wordsCount)
        {
            return phraseLenght - (wordsInPhrase - wordsCount) * 2;
        }

        private int GetMinSubPhraseLenght(int wordsCount)
        {
            return wordsCount * 2 - 1;
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

        private string[] FilterValidWords(string[] words, int maxWordLenght)
        {
            var regPattern = $"^[{string.Join("", charaters.Keys)}]*$";
            var filteredWords = words
                .Where(x => x.Length <= maxWordLenght)
                .Where(x => Regex.IsMatch(x, regPattern))
                .Where(x => charaters.All(a => x.Count(b => b == a.Key) <= a.Value));
            var uniqueWords = new HashSet<string>(filteredWords);

            return uniqueWords.ToArray();
        }

        private string GetPhrase(int[] indexes)
        {
            return string.Join(" ", indexes.Select(x => words[x]));
        }

        private void IncreaseIndexes(int[] indexes, int index)
        {
            indexes[index]++;
            for (var i = indexes.Length-1; i >= 0; i--)
            {
                if (indexes[i] >= WordsInListCount-1)
                {
                    indexes[i] = 0;
                    if (i - 1 >= 0)
                    {
                        indexes[i - 1]++;
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
