using System.Text.RegularExpressions;

namespace ImageHosts.Handlers.unsupported
{
    //href: https://fapopedia.net/lauren_summer-nude-leaks/158706.html
    //preview https://fapopedia.net/photos/l/a/lauren_summer/1000/t_0262.jpg
    //link https://fapopedia.net/photos/l/a/lauren_summer/1000/0262.jpg
    public class FapopediaImgHost
    {
        public const String DomainName = "fapopedia.net";
        private const String PreviewSrcPattern =
            "(https?://fapopedia.net/photos/[a-zA-Z0-9\\.%_~-]*/[a-zA-Z0-9\\.%_~-]*/[a-zA-Z0-9\\.%_~-]*/[0-9]*)/([a-zA-Z0-9\\.%_~-]*)_([a-zA-Z0-9\\.%_~-]*\\.[a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?)";

        private readonly Regex _previewSrcRegex;

        public FapopediaImgHost()
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

            return $"{previewSrcMatch.Groups[1].Value}/{previewSrcMatch.Groups[3].Value}";
        }
    }
}
