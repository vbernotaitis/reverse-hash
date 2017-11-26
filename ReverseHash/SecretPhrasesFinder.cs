﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseHash
{
    public class SecretPhrasesFinder
    {
        private readonly BlockingCollection<string> _phrasesToCheck;
        private readonly ConcurrentBag<string> _results;
        private readonly PhraseGenerator _phraseGenerator;
        private readonly string[] _hashes;
        private bool _isFinished;

        public SecretPhrasesFinder(PhraseGenerator phraseGenerator, string[] hashes)
        {
            _phraseGenerator = phraseGenerator;
            _hashes = hashes;
            _phrasesToCheck = new BlockingCollection<string>(1);
            _results = new ConcurrentBag<string>();
        }

        public List<string> FindSecretPhrases() {

            var phrasesGeneratorTasks = GetPhrasesGeneratorTasks(5);
            var phrasesCheckerTasks = GetPhrasesCheckerTasks(1);

            Task.WaitAll(phrasesGeneratorTasks.ToArray());
            _isFinished = true;
            Task.WaitAll(phrasesCheckerTasks.ToArray());

            return _results.ToList();
        }

        private List<Task> GetPhrasesGeneratorTasks(int tasksCount)
        {
            var tasks = new List<Task>();
            var wordsCount = _phraseGenerator.WordsInListCount;
            var lastWordsIndex = wordsCount - 1;
            var counter = 0;
            var step = Math.Max(1, _phraseGenerator.WordsInListCount / tasksCount);
            
            for (var i = 0; i < tasksCount && counter + step < lastWordsIndex; i++)
            {
                var from = counter;
                var to = from + step;
                Console.WriteLine($"{from} {to}");
                tasks.Add(Task.Run(() => PutPhrasesToQueue(from, to)));
                counter = to + 1;
            }

            Console.WriteLine($"{counter} {lastWordsIndex}");
            tasks.Add(Task.Run(() => PutPhrasesToQueue(counter, lastWordsIndex)));
            return tasks;
        }

        private void PutPhrasesToQueue(int startIndex, int endIndex)
        {
            foreach (var phrase in _phraseGenerator.GetUniquePhrases(startIndex, endIndex))
            {
                if (_isFinished) break;
                _phrasesToCheck.Add(phrase);
            }
        }

        private List<Task> GetPhrasesCheckerTasks(int tasksCount)
        {
            var tasks = new List<Task>();
            for (var i = 0; i < tasksCount; i++)
            {
                tasks.Add(Task.Run(() => CheckIfPhraseMatchHashes()));
            }
            return tasks;
        }

        private void CheckIfPhraseMatchHashes()
        {
            var hashChecker = new HashChecker();
            while (!_isFinished && _results.Count < _hashes.Length || _phrasesToCheck.Any())
            {
                while (_phrasesToCheck.TryTake(out var phrase))
                {
                    Console.WriteLine(phrase);
                    var mathingHash = hashChecker.GetMatchingHash(phrase, _hashes);
                    if (mathingHash != default(string))
                    {
                        _results.Add($"{phrase} {mathingHash}");
                    }
                }
            }
            _isFinished = _results.Count >= _hashes.Length || _isFinished;
        }
    }
}
