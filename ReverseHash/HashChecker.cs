using System.Text;
using System.Security.Cryptography;
using System.Linq;

namespace ReverseHash
{
    public class HashChecker
    {
        private MD5 md5Hash;

        public HashChecker()
        {
            md5Hash = MD5.Create();
        }

        public bool IsHashesMatch(string phrase, string[] hashes)
        {
            var phraseHash = GetMD5Hash(phrase);
            return hashes.Contains(phraseHash);
        }

        private string GetMD5Hash(string phrase)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(phrase));
            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
