using System.Collections;
using System.Collections.Generic;

namespace ReverseHash
{
    internal class IndexesEnumerable : IEnumerable<int[]>
    {
        private readonly int _maxValue;
        private readonly int[] _startIndex;
        private readonly int[] _endIndex;

        public IndexesEnumerable(int maxValue, int[] startIndex, int[] endIndex)
        {
            _startIndex = startIndex;
            _endIndex = endIndex;
            _maxValue = maxValue;
        }

        public IEnumerator<int[]> GetEnumerator()
        {
            var currentIndex = _startIndex;
            yield return currentIndex;
            while (!IsGreaterOrEqual(currentIndex, _endIndex)) {

                IncreaseIndex(currentIndex);
                yield return currentIndex;
            }
        }

        private bool IsGreaterOrEqual(int[] a, int[] b) {
            for (var i = 0; i < a.Length; i++) {
                if (a[i] > b[i])
                {
                    return true;
                }
                else if (a[i] < b[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void IncreaseIndex(int[] index)
        {
            index[index.Length - 1]++;
            for (var i = index.Length - 1; i > 0; i--)
            {
                if (index[i] > _maxValue) {
                    IncreasePreviousIndex(index, i);
                }
            }
        }

        private void IncreasePreviousIndex(int[] index, int i)
        {
            index[i] = 0;
            index[i - 1]++;
            if (index[i - 1] > _maxValue) {
                IncreasePreviousIndex(index, i - 1);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
