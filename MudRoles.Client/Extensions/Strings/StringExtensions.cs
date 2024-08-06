using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Nextended.Core.Extensions;

namespace MudRoles.Client.Extensions.Strings
{
    public static class StringExtensions
    {
        public static int CountWords(this string str)
        {
            return str.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
        public static string ToTitleCase(this string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
        public static int CountCharacters(this string str, char c)
        {
            return str.Count(ch => ch == c);
        }
        public static string RemoveWhitespace(this string str)
        {
            return new string(str.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        /// <summary>
        /// Formats the string according to the specified mask
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="mask">The mask for formatting. Like "A##-##-T-###Z"</param>
        /// <returns>The formatted string</returns>
        public static string FormatWithMask(this string input, string mask)
        {
            if (input.IsNullOrEmpty()) return input;
            var output = string.Empty;
            var index = 0;
            foreach (var m in mask)
            {
                if (m == '#')
                {
                    if (index < input.Length)
                    {
                        output += input[index];
                        index++;
                    }
                }
                else
                    output += m;
            }
            return output;
        }
        /// <summary>
        /// Strips an HTML string fragment from all dom elements and returns a plain string
        /// </summary>
        /// <param name="input">The Html string</param>
        /// <returns></returns>
        public static string StripHtml(this string input)
        {
            // Will this simple expression replace all tags???
            var tagsExpression = new Regex(@"</?.+?>");
            return tagsExpression.Replace(input, " ");
        }
        public static string ToCamelCase(this string the_string)
        {
            if (the_string == null || the_string.Length < 2)
                return the_string ?? string.Empty;

            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }
        /// <summary>
        /// Checks the given string and verifies it is a well-formed email.
        /// </summary>
        /// <param name="s">The string you need to check</param>
        /// <returns></returns>
        public static bool IsValidEmailAddress(this string s)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(s);
        }
        public static bool IsValidUrl(this string text)
        {
            Regex rx = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            return rx.IsMatch(text);
        }
        public static bool IsNotNullOrEmpty(this string input)
        {
            return !String.IsNullOrEmpty(input);
        }
        public static IEnumerable<string> SplitCamelCase(this string source)
        {
            const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
            var matches = Regex.Matches(source, pattern);
            foreach (Match match in matches)
            {
                yield return match.Value;
            }
        }
        /// <summary>
        /// Strip a string of the specified character.
        /// </summary>
        /// <param name="s">the string to process</param>
        /// <param name="char">character to remove from the string</param>
        /// <example>
        /// string s = "abcde";
        /// 
        /// s = s.Strip('b');  //s becomes 'acde;
        /// </example>
        /// <returns></returns>
        public static string Strip(this string s, char character)
        {
            s = s.Replace(character.ToString(), "");

            return s;
        }

        /// <summary>
        /// Strip a string of the specified characters.
        /// </summary>
        /// <param name="s">the string to process</param>
        /// <param name="chars">list of characters to remove from the string</param>
        /// <example>
        /// string s = "abcde";
        /// 
        /// s = s.Strip('a', 'd');  //s becomes 'bce;
        /// </example>
        /// <returns></returns>
        public static string Strip(this string s, params char[] chars)
        {
            foreach (char c in chars)
            {
                s = s.Replace(c.ToString(), "");
            }

            return s;
        }
        /// <summary>
        /// Strip a string of the specified substring.
        /// </summary>
        /// <param name="s">the string to process</param>
        /// <param name="subString">substring to remove</param>
        /// <example>
        /// string s = "abcde";
        /// 
        /// s = s.Strip("bcd");  //s becomes 'ae;
        /// </example>
        /// <returns></returns>
        public static string Strip(this string s, string subString)
        {
            s = s.Replace(subString, "");

            return s;
        }
    }

}
