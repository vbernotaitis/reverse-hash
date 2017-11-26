using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReverseHash
{
    class Program
    {
        private static bool isNotFinished = true;

        static string[] GetHashesList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        static string[] GetWordsList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        static void WriteResult(string path, string[] result)
        {
            System.IO.File.WriteAllLines(path, result);
        }

        static void PutPhrasesToQueue(BlockingCollection<string> allPhrases, PhraseGenerator phraseGenerator, int startIndex, int endIndex)
        {
            foreach (var phrase in phraseGenerator.GetUniquePhrases(startIndex, endIndex))
            {
                if (!isNotFinished) break;
                allPhrases.Add(phrase);
            }
            Console.WriteLine("THREAD IS FINISHED");
        }

        static void CheckIfMatch(BlockingCollection<string> phrases, string[] hashes, List<string> result)
        {
            var hashChecker = new HashChecker();
            while (result.Count < hashes.Length && isNotFinished || phrases.Any()) {
                var phrase = string.Empty;
                while (phrases.TryTake(out phrase))
                {
                    Console.WriteLine(phrase);
                    if (hashChecker.IsHashesMatch(phrase, hashes))
                    {
                        result.Add(phrase);
                        Console.WriteLine($"FOUND IT: {phrase}");
                    }
                }
            }
            isNotFinished = isNotFinished || result.Count < hashes.Length;
        }

        private static List<Task> GetPutPrashesTasks(int tasksCount, BlockingCollection<string> allPhrases, PhraseGenerator phraseGenerator)
        {
            int counter = 0;
            var step = phraseGenerator.WordsInListCount / tasksCount;
            var tasks = new List<Task>();
            for (var i = 0; i < tasksCount; i++)
            {
                var from = counter;
                var to = from + step;
                tasks.Add(Task.Run(() => PutPhrasesToQueue(allPhrases, phraseGenerator, from, to)));
                counter += step;
            }
            return tasks;
        }

        static void Main(string[] args)
        {
            var anagram = "poultry outwits ants";
            var resultPath = @"D:\result";
            var hashes = GetHashesList(@"D:\hash");
            var words = GetWordsList(@"D:\wordlist");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var phraseGenerator = new PhraseGenerator(anagram, words);
            var allPhrases = new BlockingCollection<string>(1);
            var result = new List<string>();

            var getPhrasesTasks = GetPutPrashesTasks(6, allPhrases, phraseGenerator);
            var checkerThread1 = new Thread(() => CheckIfMatch(allPhrases, hashes, result));
            checkerThread1.Start();

            Task.WaitAll(getPhrasesTasks.ToArray());
            isNotFinished = false;

            stopWatch.Stop();
            result.Add(stopWatch.Elapsed.TotalMinutes.ToString());

            WriteResult(resultPath, result.ToArray());
        }
    }
}
