using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ReverseHash
{
    public class HashChecker
    {
        private readonly MD5 _md5Hash;

        public HashChecker()
        {
            _md5Hash = MD5.Create();
        }

        public string GetMatchingHash(string phrase, string[] hashes)
        {
            var phraseHash = GetMd5Hash(phrase);
            return hashes.SingleOrDefault(x => x.Equals(phraseHash));
        }

        private string GetMd5Hash(string phrase)
        {
            var data = _md5Hash.ComputeHash(Encoding.UTF8.GetBytes(phrase));
            var hash = string.Join("", data.Select(x => x.ToString("x2")));
            return hash;
        }
    }
}