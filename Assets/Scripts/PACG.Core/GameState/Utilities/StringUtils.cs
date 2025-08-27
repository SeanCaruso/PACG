
namespace PACG.Core
{
    public static class StringUtils
    {
        public static string ReplaceAdventureLevel(string text, int adventureNumber)
        {
            if (string.IsNullOrEmpty(text) || !text.Contains('#'))
                return text;

            // This regular expression matches two patterns:
            // 1. (\d+)\+(#+): This captures a number, followed by a literal '+', and then one or more '#' characters.
            //                 The number is in group 1 and the '#'s are in group 2.
            // 2. |#+: This is an OR condition, matching one or more '#' characters by themselves.
            return System.Text.RegularExpressions.Regex.Replace(text, @"(\d+)\+(#+)|#+", match =>
            {
                // Check if the first capturing group (the number before "+#") was successful.
                if (match.Groups[1].Success)
                {
                    // This block executes for matches like "5+#" or "10+##".
                    var baseNumber = int.Parse(match.Groups[1].Value);
            
                    // The number of '#' characters is the length of the second captured group.
                    var hashCount = match.Groups[2].Length;

                    var value = baseNumber + (adventureNumber * hashCount);
                    return value.ToString();
                }
                else
                {
                    // This block executes for matches of only '#' characters, preserving the original functionality.
                    var count = match.Length;
                    var value = adventureNumber * count;
                    return value.ToString();
                }
            });
        }
    }
}
