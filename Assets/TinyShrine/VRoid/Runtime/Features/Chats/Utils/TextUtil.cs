using System.Text.RegularExpressions;

namespace TinyShrine.VRoid.Chats.Utils
{
    public static class TextUtil
    {
        private static readonly Regex BoldRegex = new(@"(\*\*|__)(.*?)\1", RegexOptions.Compiled);
        private static readonly Regex ItalicRegex = new(@"(\*|_)(.*?)\1", RegexOptions.Compiled);

        public static string ConvertMarkdownToTMPRichText(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
            {
                return string.Empty;
            }

            // Convert bold (**text** or __text__) to <b>text</b>
            var tmpRichText = BoldRegex.Replace(markdown, "<b>$2</b>");

            // Convert italics (*text* or _text_) to <i>text</i>
            return ItalicRegex.Replace(tmpRichText, "<i>$2</i>");
        }
    }
}
