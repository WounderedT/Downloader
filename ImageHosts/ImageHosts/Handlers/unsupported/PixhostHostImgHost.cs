using System.Text.RegularExpressions;

namespace ImageHosts.Handlers.unsupported
{
    //href: https://pixhost.to/show/89/114671009_1.jpg
    //preview https://t33.pixhost.to/thumbs/89/114671009_1.jpg
    //link https://img33.pixhost.to/images/89/114671009_1.jpg
    public class PixhostHostImgHost
    {
        public const String DomainName = "pixhost.to";
        private const String PreviewSrcPattern =
            "^https://t([0-9]*).pixhost.to/[a-zA-Z0-9\\.%_~-]*/([0-9]*/[a-zA-Z0-9\\.%_~-]*.[a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?)$";

        private readonly Regex _previewSrcRegex;

        public PixhostHostImgHost()
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

            return $"https://img{previewSrcMatch.Groups[1].Value}.pixhost.to/images/{previewSrcMatch.Groups[2].Value}";
        }
    }
}
