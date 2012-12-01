
namespace SV
{
    using System;

    public static class StringExtensions
    {
        public static string F(this string st, params object[] args)
        {
            return String.Format(st, args);
        }

        public static string[] SplitIntoLines(this string text)
        {
            return text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitIntoLines(this string text, int maxNumberOfLines)
        {
            return text.Split(new char[] { '\r', '\n' }, maxNumberOfLines, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
