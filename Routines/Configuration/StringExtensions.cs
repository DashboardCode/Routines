namespace DashboardCode.Routines.Configuration
{
    public static class StringExtensions
    {
        public const string Asterix = "*";

        public static bool IsLetterOrUnderscore(this char c) => 
            (char.IsLetter(c) || c == '_');

        public static string ReplaceEmptyWithAsterix(this string text) =>
            string.IsNullOrWhiteSpace(text) ? Asterix : text.Trim();

        public static bool IsNullOrWhiteSpaceOrAsterix(this string text) =>
            string.IsNullOrWhiteSpace(text) || text.Trim() == Asterix;
        
        public static bool AsterixEquals(string text1, string text2) =>
             (text1 == text2 || (IsNullOrWhiteSpaceOrAsterix(text1) && IsNullOrWhiteSpaceOrAsterix(text2)));
    }
}
