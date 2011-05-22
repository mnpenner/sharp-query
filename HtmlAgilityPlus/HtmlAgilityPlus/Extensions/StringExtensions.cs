using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlAgilityPlus.Extensions
{
    internal static class StringExtensions
    {
        internal static IEnumerable<KeyValuePair<char?, string>> SplitOn(this string input, params char[] separators)
        {
            int startIndex = 0;
            var state = new Stack<char>();
            input = input.Trim(separators);

            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];
                char s = state.Count > 0 ? state.Peek() : default(char);

                if (state.Count > 0 && (s == '\\' || (s == '[' && c == ']') || ((s == '"' || s == '\'') && c == s)))
                    state.Pop();
                else if (c == '\\' || c == '[' || c == '"' || c == '\'')
                    state.Push(c);
                else if (state.Count == 0 && separators.Contains(c))
                {
                    char key = c;
                    int endIndex = i;
                    while (separators.Contains(input[++i])) if (input[i] != ' ') key = input[i];
                    yield return new KeyValuePair<char?, string>(key, input.Substring(startIndex, endIndex - startIndex));
                    startIndex = i;
                }
            }

            yield return new KeyValuePair<char?, string>(null, input.Substring(startIndex));
        }

        internal static string StripSlashes(this string input)
        {
            return Regex.Replace(input, @"\\([""'\\/])", "$1");
        }

        internal static IEnumerable<string> SplitBefore(this string input, params char[] separators)
        {
            int start = 0;
            char? state = null;

            for (int i = 0; i < input.Length; ++i)
            {
                if (state == null && separators.Contains(input[i]))
                {
                    yield return input.Substring(start, i - start);
                    start = i;
                } 
                else if(state == null && (input[i] == '\\' || input[i] == '"' || input[i] == '\'' || input[i] == '/'))
                {
                    state = input[i];
                } 
                else if(state == '\\' || ((state == '"' || state == '\'' || state == '/') && state == input[i]))
                {
                    state = null;
                }
            }

            yield return input.Substring(start);
        }

        internal static string Slice(this string str, int? start = null, int? end = null, int step = 1)
        {
            if (step == 0) throw new ArgumentException("Step size cannot be zero.", "step");

            if (start == null)
            {
                if (step > 0) start = 0;
                else start = str.Length - 1;
            }
            else if (start < 0)
            {
                if (start < -str.Length) start = 0;
                else start += str.Length;
            }
            else if (start > str.Length) start = str.Length;

            if (end == null)
            {
                if (step > 0) end = str.Length;
                else end = -1;
            }
            else if (end < 0)
            {
                if (end < -str.Length) end = 0;
                else end += str.Length;
            }
            else if (end > str.Length) end = str.Length;

            if (start == end || start < end && step < 0 || start > end && step > 0) return "";
            if (start < end && step == 1) return str.Substring((int)start, (int)(end - start));

            int length = (int)(((end - start) / (float)step) + 0.5f);
            var sb = new StringBuilder(length);
            for (int i = (int)start, j = 0; j < length; i += step, ++j)
                sb.Append(str[i]);
            return sb.ToString();
        }
    }
}