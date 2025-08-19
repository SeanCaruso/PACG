
namespace PACG.Core
{
    public static class StringUtils
    {
        public static string ReplaceAdventureLevel(string text, int adventureNumber)
        {
            if (string.IsNullOrEmpty(text) || !text.Contains('#'))
                return text;
            
            return System.Text.RegularExpressions.Regex.Replace(text, @"#+", match =>
            {
                var count = match.Length;
                var value = adventureNumber * count;
                return value.ToString();
            });
        }
    }
}
