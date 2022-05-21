using DomainProcessors.Extensions.UI.Interfaces;
using Microsoft.Extensions.Logging;
using ProgressHandler.Abstractions;
using Shared.Models.Work;
using System.Text.RegularExpressions;

namespace DomainProcessors.Extensions.UI.Processors
{
    public class NcfProcessor : IDomainUIProcessor
    {
        public const String DomainName = "www.nudecelebforum.com";
        public String Domain { get; } = DomainName;
        private const String DownloadTip = "If some images are missing try signing in.";

        private const Char SnippetResultSeparator = ';';
        private const String DomainPattern = "https?://([a-zA-Z0-9\\.%_~-]*)/*";
        private const String GetImagesByPostSnippet = @"
var links = [];
var posts = document.querySelectorAll('[id^=""post_message_""]');
var collection;
var postIndex;
var index;
for (postIndex = 0; postIndex < posts.length; ++postIndex) {
	collection = posts[postIndex].getElementsByTagName(""a"");
	for (index = 0; index < collection.length; ++index) {
        if(collection[index].getElementsByTagName(""img"")[0] != undefined){
            links.push("""".concat(postIndex, "";"", collection[index].href, "";"" , collection[index].getElementsByTagName(""img"")[0].src));
        }
	}
}
links.toString();
";
        private readonly Regex _domainRegex;
        private readonly ILogger<NcfProcessor> _logger;
        private readonly IJsProcessor _jsProcessor;
        private readonly IProgressHandler _progressHandler;
        //private readonly ImageTwistImgHost _imageTwist;
        //private readonly PixhostHostImgHost _pixhost;
        //private readonly PimpandhostImgHost _pimpandhost;
        //private readonly ImagebamImgHost _imagebam;
        //private readonly ImgboxImgHost _imgbox;
        //private readonly MotherlessmediaImgHost _motherlessmedia;

        public NcfProcessor(ILogger<NcfProcessor> logger, IJsProcessor jsProcessor, IProgressHandler progressHandler)
        {
            _jsProcessor = jsProcessor;
            _domainRegex = new Regex(DomainPattern);
            _progressHandler = progressHandler;
            //_imageTwist = new ImageTwistImgHost();
            //_pixhost = new PixhostHostImgHost();
            //_pimpandhost = new PimpandhostImgHost();
            //_imagebam = new ImagebamImgHost();
            //_imgbox = new ImgboxImgHost();
            //_motherlessmedia = new MotherlessmediaImgHost();
        }

        #region Implementation of IDomainUIProcessor

        /// <inheritdoc />
        public Regex DomainRegex { get; } = new Regex(DomainName);

        /// <inheritdoc />
        public Object WebViewObj { get; set; }

        ///<inheritdoc />
        public async Task<Int32> GetElementsCountAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //return (await GetImagesPerPost(webView)).Length;
        }

        /// <inheritdoc />
        public async Task<WorkRequest> ProcessAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //var result = new ProcessResult();
            //var imagesPerPost = await GetImagesPerPost(webView);
            //String[] tmp;
            //foreach (var image in imagesPerPost)
            //{
            //    tmp = image.Split(SnippetResultSeparator);
            //    switch (GetDomain(tmp[1]))
            //    {
            //        case ImagebamImgHost.DomainName:
            //            result.Add(await _imagebam.BuildImageUrlAsync(tmp[1]));
            //            break;
            //        case ImgboxImgHost.DomainName:
            //            result.Add(await _imgbox.BuildImageUrlAsync(tmp[1]));
            //            break;
            //        case ImageTwistImgHost.DomainName:
            //            result.Add(_imageTwist.BuildImageUrl(tmp[1], tmp[2]));
            //            break;
            //        case PixhostHostImgHost.DomainName:
            //            result.Add(_pixhost.BuildImageUrl(tmp[2]));
            //            break;
            //        case PimpandhostImgHost.DomainName:
            //            result.Add(_pimpandhost.BuildImageUrl(tmp[2]));
            //            break;
            //        case MotherlessmediaImgHost.DomainName:
            //            result.Add(_motherlessmedia.BuildImageUrl(tmp[2]));
            //            break;
            //        default:
            //            _logger.LogWarning($"Unsupported image host found. Domain [{GetDomain(tmp[1])}] doesn't have implementation");
            //            throw new ProcessingException($"Unsupported image host found. Domain [{GetDomain(tmp[1])}] doesn't have implementation");
            //    }
            //    progressHdlr.Update();
            //}

            //return result;
        }

        #endregion

        private async Task<String[]> GetImagesPerPost(Object webView)
        {
            throw new NotImplementedException();
            //var result = await webView.InvokeScriptAsync("eval", new[] { GetImagesByPostSnippet });
            //return result.Split(ScriptResultSeparator, StringSplitOptions.RemoveEmptyEntries);
        }

        private String? GetDomain(String url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            Match? match = _domainRegex.Match(url);
            return match.Success ? match.Groups[1].Value : null;
        }

        public Task<WorkRequest> ProcessAsync(IEnumerable<String> urls, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
