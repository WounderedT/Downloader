using System.Text.RegularExpressions;

namespace ImageHosts.Handlers.unsupported
{
    //href: https://nudostar.com/forum/attachments/135858319_13-jpg.246258/
    //preview https://nudostar.com/forum/data/attachments/246/246258-e7efe6da62d6bde195eeee872d27bb3c.jpg
    //link https://nudostar.com/forum/attachments/135858319_13-jpg.246258/
    public class NudostarImgHost
    {
        public const String DomainName = "nudostar.com";
        private const String HrefPattern =
            "^(https?://nudostar.com/forum/attachments/[\\u0000-\\u024F]*|[0-9\\\\.()%_~\\\\[\\\\]-]*)-([a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1}))?\\.[0-9]*/?$";

        private readonly Regex _hrefRegex;

        public NudostarImgHost()
        {
            _hrefRegex = new Regex(HrefPattern);
        }

        //public ProcessResultEntity ProcessHref(String href)
        //{
        //    Match? hrefMatch = _hrefRegex.Match(href);
        //    if (!hrefMatch.Success)
        //    {
        //        throw new ArgumentException($"{nameof(hrefMatch)} [{href}] does not match expected pattern [{HrefPattern}]");
        //    }

        //    return new ProcessResultEntity
        //    {
        //        Filename = $"{hrefMatch.Groups[2].Value}",
        //        Url = new Uri(href)
        //    };
        //}
    }
}
