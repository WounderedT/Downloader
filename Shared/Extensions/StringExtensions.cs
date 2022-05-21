using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Shared.Extensions
{
    /// <summary>
    /// Type provides useful string extension methods
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex _whitespaceRegex = new Regex("^\\s{1,}$");

        /// <summary>
        /// Capitalizes first letter in the provided <paramref name="input"/>
        /// </summary>
        /// <param name="input">String to capitalize letter in</param>
        /// <returns>A new string with capital first letter</returns>
        /// <exception cref="ArgumentNullException">Input string is NULL</exception>
        /// <exception cref="ArgumentException">Input string is empty consists only of white-space characters</exception>
        public static String Capitalize(this String input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (String.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException($"{nameof(input)} cannot be empty or consists only of white-space characters", nameof(input));
            }
            return String.Concat(input.First().ToString().ToUpper(), input.AsSpan(1));
        }

        /// <summary>
        /// Decodes ASCII URL string to UTF-8 string.
        /// </summary>
        /// <param name="str">URL string to decode.</param>
        /// <returns>A new string with all non-ASCII characters decoded to corresponding UTF-8 characters.</returns>
        /// <exception cref="ArgumentNullException">Input string is NULL or empty</exception>
        public static String DecodeUrlString(this String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(nameof(str));
            }
            return System.Web.HttpUtility.UrlDecode(str, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes object <typeparamref name="T"/> from JSON string representation <paramref name="str"/>
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to</typeparam>
        /// <param name="str">A JSON string represenatation of an object</param>
        /// <returns>A new instanse of <typeparamref name="T"/> from JSON string representation</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is NULL</exception>
        public static T? FromJsonString<T>(this String str)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(nameof(str));
            }
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
