using System.Text;

namespace Toolset.Core
{
    /// <summary>
    /// Extensions for the string class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Uses a string builder to format the string. Should be used instead of string.Format() because
        /// of the immutability of strings vs mutability of stringbuilder.
        /// </summary>
        /// <param name="str">The source string to format.</param>
        /// <param name="formattingParams">The parameters to be injected into the string while formatting</param>
        /// <returns>The result string</returns>
        public static string StringBuilderFormat(this string str, params object[] formattingParams)
        {
            if (str.IsNullOrWhiteSpace())
                return str;

            StringBuilder stringBuilder = new StringBuilder(string.Empty);
            stringBuilder.AppendFormat(str, formattingParams);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Uses a string builder to append multiple objects to the string. 
        /// Should be used instead of += if appending several objects to the string.
        /// </summary>
        /// <param name="str">The source string to append to.</param>
        /// <param name="formattingParams">The ordered objects to be added to the string.</param>
        /// <returns>The result string</returns>
        public static string StringBuilderAppend(this string str, params object[] objectsToAppend)
        {
            if (str.IsNullOrWhiteSpace())
                return str;

            StringBuilder stringBuilder = new StringBuilder(str);
            for (int i = 0; i < objectsToAppend.Length; ++i)
            {
                stringBuilder.Append(objectsToAppend[i]);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Extension method wrapper around string.IsNullOrWhiteSpace used for
        /// syntactical convenience.
        /// </summary>
        /// <param name="str">The string to check if null or white space.</param>
        /// <returns>Whether or not the string is null or whitespace.</returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}