using System.Text.RegularExpressions;

namespace ImageHosts.Handlers.unsupported
{
    //href: https://motherless.com/DC48134?utm_source=nudecelebforum&utm_campaign=fr-1&utm_medium=thumb
    //preview https://cdn5-thumbs.motherlessmedia.com/thumbs/DC48134.jpg?from_helper
    //link https://cdn5-images.motherlessmedia.com/images/DC48134.jpg
    public class MotherlessmediaImgHost
    {
        public const String DomainName = "motherless.com";
        private const String PreviewSrcPattern =
            "(https?://[a-zA-Z0-9]*-)thumbs(.motherlessmedia.com/thumbs/[a-zA-Z0-9\\.%_~-]*)(\\.[a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?)\\?from_helper";

        private readonly Regex _previewSrcRegex;

        public MotherlessmediaImgHost()
        {
            _previewSrcRegex = new Regex(PreviewSrcPattern);
        }

        public String BuildImageUrl(String previewSrc)
        {
            if (String.IsNullOrEmpty(previewSrc))
            {
                throw new ArgumentNullException(nameof(previewSrc));
            }

            Match? previewSrcMatch = _previewSrcRegex.Match(previewSrc);
            if (!previewSrcMatch.Success)
            {
                throw new ArgumentException($"{nameof(previewSrcMatch)} [{previewSrc}] does not match expected pattern [{PreviewSrcPattern}]");
            }

            return $"{previewSrcMatch.Groups[1].Value}images{previewSrcMatch.Groups[2].Value}{previewSrcMatch.Groups[3].Value}";
        }
    }
}
