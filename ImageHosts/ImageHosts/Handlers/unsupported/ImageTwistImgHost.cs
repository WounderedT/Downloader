using System.Text.RegularExpressions;

namespace ImageHosts.Handlers.unsupported
{
    //href https://imagetwist.com/atr8xfkflaks/Tanya_Frost_1-01.jpg
    //preview https://img68.imagetwist.com/th/29384/atr8xfkflaks.jpg
    //link https://img68.imagetwist.com/i/29384/atr8xfkflaks.jpg/Tanya_Frost_1-01.jpg
    public class ImageTwistImgHost
    {
        public const String DomainName = "imagetwist.com";
        private const String HrefPattern = "^https?://imagetwist.com/[a-zA-Z0-9]*/([a-zA-Z0-9\\.%_~-]*.[a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?)$";
        private const String PreviewSrcPattern =
            "^(https?://[a-zA-Z0-9]*.imagetwist.com)/[a-zA-Z0-9\\.%_~-]*/([0-9]*)/([a-zA-Z0-9\\.%_~-]*.[a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?)$";

        private readonly Regex _hrefRegex;
        private readonly Regex _previewSrcRegex;

        public ImageTwistImgHost()
        {
            _hrefRegex = new Regex(HrefPattern);
            _previewSrcRegex = new Regex(PreviewSrcPattern);
        }

        public String BuildImageUrl(String href, String previewSrc)
        {
            if (String.IsNullOrEmpty(href))
            {
                throw new ArgumentNullException(nameof(href));
            }
            if (String.IsNullOrEmpty(previewSrc))
            {
                throw new ArgumentNullException(nameof(previewSrc));
            }

            Match? hrefMatch = _hrefRegex.Match(href);
            if (!hrefMatch.Success)
            {
                throw new ArgumentException($"{nameof(href)} [{href}] does not match expected pattern [{HrefPattern}]");
            }

            Match? previewSrcMatch = _previewSrcRegex.Match(previewSrc);
            if (!previewSrcMatch.Success)
            {
                throw new ArgumentException($"{nameof(previewSrcMatch)} [{previewSrc}] does not match expected pattern [{PreviewSrcPattern}]");
            }

            return $"{previewSrcMatch.Groups[1].Value}/i/{previewSrcMatch.Groups[2].Value}/{previewSrcMatch.Groups[3].Value}/{hrefMatch.Groups[1].Value}";
        }
    }
}
