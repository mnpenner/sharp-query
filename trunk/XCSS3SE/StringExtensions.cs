using System;
using System.Text;

namespace XCSS3SE
{
    internal static class StringExtensions
    {
        public static string Slice(this string str, int? start = null, int? end = null, int step = 1)
        {
            if (step == 0) throw new ArgumentException("Step size cannot be zero", "step");

            if (start == null) start = step > 0 ? 0 : str.Length - 1;
            else if (start < 0) start = start < -str.Length ? 0 : str.Length + start;
            else if (start > str.Length) start = str.Length;

            if (end == null) end = step > 0 ? str.Length : -1;
            else if (end < 0) end = end < -str.Length ? 0 : str.Length + end;
            else if (end > str.Length) end = str.Length;

            if (start == end || start < end && step < 0 || start > end && step > 0) return "";
            if (step == 1) return str.Substring(start.Value, end.Value - start.Value);

            var sb = new StringBuilder((int)Math.Ceiling((end - start).Value / (float)step));
            for (int i = start.Value; step > 0 && i < end || step < 0 && i > end; i += step)
                sb.Append(str[i]);
            return sb.ToString();
        }
    }
}