namespace Vse.Routines.Configuration
{
    static class StringExtensions
    {
        public const string Asterix = "*";

        public static bool IsLetterOrUnderscore(this char c)
        {
            if (char.IsLetter(c) || c == '_')
                return true;
            return false;
        }
        public static string ReplaceEmptyWithAsterix(this string text)
        {
            string @value = default(string);
            if (string.IsNullOrWhiteSpace(text))
                @value = Asterix;
            else
                @value = text.Trim();
            return @value;
        }
        //public static bool IsAsterix(this string text)
        //{
        //    return !string.IsNullOrEmpty(text) && text.Trim() == Asterix;
        //}
        public static bool IsNullOrWhiteSpaceOrAsterix(this string text)
        {
            return string.IsNullOrWhiteSpace(text) || text.Trim() == Asterix;
        }
    }
}
