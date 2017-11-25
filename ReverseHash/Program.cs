using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ReverseHash
{
    class Program
    {
        static string[] GetHashesList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        static string[] GetWordsList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        static void WriteResult(string path, string result)
        {
            System.IO.File.WriteAllText(path, result);
        }

        static void PutPhrasesToQueue(BlockingCollection<string> allPhrases, PhraseGenerator phraseGenerator, int startIndex, int endIndex)
        {
            foreach (var phrase in phraseGenerator.GetUniquePhrases(startIndex, endIndex))
            {
                allPhrases.Add(phrase);
            }
        }

        static void CheckIfMatch(BlockingCollection<string> phrases, string[] hashes, ref bool isFound)
        {
            var hashChecker = new HashChecker();
            while (!isFound) {
                var phrase = string.Empty;
                while (phrases.TryTake(out phrase))
                {
                    Console.WriteLine(phrase);
                    if (hashChecker.IsHashesMatch(phrase, hashes))
                    {
                        isFound = true;
                        WriteResult(@"D:\result", phrase);
                        Console.WriteLine($"FOUND IT: {phrase}");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var anagram = "poultry outwits ants";
            var resultPath = @"D:\result";
            var hashes = GetHashesList(@"D:\hash");
            var words = GetWordsList(@"D:\wordlist");

            var phraseGenerator = new PhraseGenerator(anagram, words);
            var allPhrases = new BlockingCollection<string>(1);
            var isFound = false;

            var start = 0;
            int middle = phraseGenerator.WordsCount / 2;
            var end = phraseGenerator.WordsCount;

            var phrasesThread1 = new Thread(() => PutPhrasesToQueue(allPhrases, phraseGenerator, start, middle));
            var phrasesThread2 = new Thread(() => PutPhrasesToQueue(allPhrases, phraseGenerator, middle, end));

            var checkerThread1 = new Thread(() => CheckIfMatch(allPhrases, hashes, ref isFound));
            var checkerThread2 = new Thread(() => CheckIfMatch(allPhrases, hashes, ref isFound));

            phrasesThread1.Start();
            phrasesThread2.Start();
            checkerThread1.Start();
            //checkerThread2.Start();
        }
    }
}
