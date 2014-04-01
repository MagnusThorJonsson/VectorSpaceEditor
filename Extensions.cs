using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace VectorSpace
{
    static class Extensions
    {
        private const string INDENT_STRING = "\t";

        #region Point Extensions
        /// <summary>
        /// Gets the distance between two points
        /// </summary>
        /// <param name="p">Point A</param>
        /// <param name="q">Point B</param>
        /// <returns>Distance between the points</returns>
        public static double GetDistance(this Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);

            return distance;
        }
        #endregion


        #region Collection Extensions
        /// <summary>
        /// Sorting helper for ObservableCollections
        /// </summary>
        /// <typeparam name="TSource">Source item</typeparam>
        /// <typeparam name="TKey">Search parameters</typeparam>
        /// <param name="source">Source item initiator</param>
        /// <param name="keySelector">Search parameter</param>
        /// <param name="direction">Sorting direction</param>
        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector, ListSortDirection direction = ListSortDirection.Ascending)
        {
            List<TSource> sortedList = null;

            if (direction == ListSortDirection.Ascending)
                sortedList = source.OrderBy(keySelector).ToList();
            else if (direction == ListSortDirection.Descending)
                sortedList = source.OrderByDescending(keySelector).ToList();
            
            source.Clear();
            for (int i = 0; i < sortedList.Count; i++)
            {
                source.Add(sortedList[i]);
            }
        }

        /// <summary>
        /// Loops through a list or array and applies the action to each element
        /// </summary>
        /// <typeparam name="T">The object typeof</typeparam>
        /// <param name="ie">IEnumerable object</param>
        /// <param name="action">The action to apply</param>
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (T i in ie)
            {
                action(i);
            }
        }
        #endregion


        #region String Extensions
        /// <summary>
        /// Sanitizes a string into a valid filename
        /// </summary>
        /// <param name="name">The string to sanitize</param>
        /// <returns>The sanitized string</returns>
        public static string SanitizeFilename(this string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            string sanitized = System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
            
            // Trim last whitespace
            sanitized = sanitized.Trim();
            // While the last char is a . we trim it away
            while (sanitized[sanitized.Length - 1].Equals("."))
                sanitized = sanitized.TrimEnd('.');

            return sanitized;
        }


        /// <summary>
        /// Indents the JSON to a human readable format
        /// </summary>
        /// <param name="str">The string to format</param>
        /// <returns>The formatted string</returns>
        public static string IndentJson(this string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
