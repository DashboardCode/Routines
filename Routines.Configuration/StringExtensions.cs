namespace Vse.Routines.Configuration
{
    public static class StringExtensions
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
        public static bool IsNullOrWhiteSpaceOrAsterix(this string text)
        {
            return string.IsNullOrWhiteSpace(text) || text.Trim() == Asterix;
        }
        public static bool AsterixEquals(string text1, string text2)
        {
            return (text1 == text2 || (IsNullOrWhiteSpaceOrAsterix(text1) && IsNullOrWhiteSpaceOrAsterix(text2)));
        }
    }
}
