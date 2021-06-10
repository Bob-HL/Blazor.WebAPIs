using System.Security.Cryptography;
using System.Text;

namespace Cutec.Blazor.WebAPIs
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string word)
        {
            char[] wordLetters = word.ToCharArray();
            wordLetters[0] = char.ToLower(wordLetters[0]);
            var output = new string(wordLetters);
            return output;
        }
    }
}
