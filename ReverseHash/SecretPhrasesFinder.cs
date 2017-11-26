using System;
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
        private bool _isNotFinished = true;

        public SecretPhrasesFinder(PhraseGenerator phraseGenerator, string[] hashes)
        {
            _phraseGenerator = phraseGenerator;
            _hashes = hashes;
            _phrasesToCheck = new BlockingCollection<string>(1);
            _results = new ConcurrentBag<string>();
        }

        public List<string> FindSecretPhrases() {

            var phrasesGeneratorTasks = GetPhrasesGeneratorTasks(6);
            var phrasesCheckerTasks = GetPhrasesCheckerTasks(1);

            Task.WaitAll(phrasesGeneratorTasks.ToArray());
            _isNotFinished = false;
            Task.WaitAll(phrasesCheckerTasks.ToArray());

            return _results.ToList();
        }

        private List<Task> GetPhrasesGeneratorTasks(int tasksCount)
        {
            var counter = 0;
            var step = _phraseGenerator.WordsInListCount / tasksCount;
            var tasks = new List<Task>();
            for (var i = 0; i < tasksCount; i++)
            {
                var from = counter;
                var to = from + step;
                tasks.Add(Task.Run(() => PutPhrasesToQueue(from, to)));
                counter += step;
            }
            return tasks;
        }

        private void PutPhrasesToQueue(int startIndex, int endIndex)
        {
            foreach (var phrase in _phraseGenerator.GetUniquePhrases(startIndex, endIndex))
            {
                if (!_isNotFinished) break;
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
            while (_results.Count < _hashes.Length && _isNotFinished || _phrasesToCheck.Any())
            {
                while (_phrasesToCheck.TryTake(out var phrase))
                {
                    Console.WriteLine(phrase);
                    if (hashChecker.IsHashesMatch(phrase, _hashes))
                    {
                        _results.Add(phrase);
                    }
                }
            }
            _isNotFinished = _isNotFinished || _results.Count < _hashes.Length;
        }
    }
}
