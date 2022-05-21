using System.Text.RegularExpressions;

namespace ImageHosts.Handlers.unsupported
{
    //href: http://imgbox.com/LJAIxxez
    //preview https://1-t.imgbox.com/LJAIxxez.jpg
    //link https://images3.imgbox.com/3d/9e/LJAIxxez_o.jpg
    //Additional http request require to get image link!
    public class ImgboxImgHost
    {
        public const String DomainName = "imgbox.com";
        private const String ContentPattern = "meta property=\"og:image\" content=\"(.*)\"";

        private readonly HttpClient _client;
        private readonly Regex _contentRegex;

        public ImgboxImgHost()
        {
            _client = new HttpClient();
            _contentRegex = new Regex(ContentPattern);
        }

        //public async Task<String> BuildImageUrlAsync(String href)
        //{
        //    if (String.IsNullOrEmpty(href))
        //    {
        //        throw new ArgumentNullException(nameof(href));
        //    }

        //    HttpResponseMessage? hrefGetResult = await _client.GetAsync(new Uri(href));
        //    hrefGetResult.EnsureSuccessStatusCode();
        //    var content = await hrefGetResult.Content.ReadAsStringAsync();
        //    Match? match = _contentRegex.Match(content);
        //    if (!match.Success)
        //    {
        //        throw new ProcessingException($"Failed to process {nameof(href)} [{href}]: content regex return no matches");
        //    }

        //    return $"{match.Groups[1].Value}";
        //}
    }
}
