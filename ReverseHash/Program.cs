using System;
using System.Diagnostics;
using System.Globalization;

namespace ReverseHash
{
    public class Program
    {
        private static string[] GetHashesList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        private static string[] GetWordsList(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }

        private static void WriteResult(string path, string[] result)
        {
            System.IO.File.WriteAllLines(path, result);
        }

        public static void Main()
        {
            const string resultPath = @".\datafiles\result";

            const string anagram = "poultry outwits ants";
            var hashes = GetHashesList(@".\datafiles\hashes");
            var words = GetWordsList(@".\datafiles\wordlist");

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var phraseGenerator = new PhraseGenerator(anagram, words);
            var tasksManager = new SecretPhrasesFinder(phraseGenerator, hashes);
            var result = tasksManager.FindSecretPhrases();

            stopWatch.Stop();

            result.Add(stopWatch.Elapsed.TotalMinutes.ToString(CultureInfo.InvariantCulture));

            Console.WriteLine("----------------------");
            Console.WriteLine(string.Join(" ", result));
            WriteResult(resultPath, result.ToArray());
        }
    }
}
