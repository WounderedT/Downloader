using System.Text.RegularExpressions;

namespace ImageHosts.Handlers.unsupported
{
    //href: http://pimpandhost.com/image/60648886
    //preview http://ist3-4.filesor.com/pimpandhost.com/1/_/_/_/1/4/6/t/x/46txQ/by_nomaxi_29_0.jpg  
    //link https://ist3-4.filesor.com/pimpandhost.com/1/_/_/_/1/4/6/t/x/46txQ/by_nomaxi_29.jpg
    public class PimpandhostImgHost
    {
        public const String DomainName = "pimpandhost.com";
        private const String PreviewSrcPattern =
            "https?://([a-zA-Z0-9\\.%_~-]*/pimpandhost.com/1/_/_/_/[a-zA-Z0-9\\.%/_~-]*/[a-zA-Z0-9\\.%_~-]*/[a-zA-Z0-9\\.%_~-]*/[a-zA-Z0-9\\.%_~-]*/[a-zA-Z0-9\\.%_~-]*)_0(\\.[a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?)";

        private readonly Regex _previewSrcRegex;

        public PimpandhostImgHost()
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

            return $"https://{previewSrcMatch.Groups[1].Value}{previewSrcMatch.Groups[2].Value}";
        }
    }
}
