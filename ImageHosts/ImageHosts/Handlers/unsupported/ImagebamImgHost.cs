using System.Text.RegularExpressions;

namespace ImageHosts.Handlers.unsupported
{
    //href: http://www.imagebam.com/image/da9262528026564
    //preview http://thumbnails117.imagebam.com/52803/da9262528026564.jpg
    //link https://images3.imagebam.com/7d/91/1a/da9262528026564.jpg
    //Additional http request require to get image link!
    public class ImagebamImgHost
    {
        public const String DomainName = "www.imagebam.com";
        private const String ContentPattern = "meta property=\"og:image\" content=\"(.*)\"";

        private readonly HttpClient _client;
        private readonly Regex _contentRegex;

        public ImagebamImgHost()
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
