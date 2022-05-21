using DomainProcessors.Extensions.UI.Interfaces;
using ProgressHandler.Abstractions;
using Shared.Models.Work;
using System.Text.RegularExpressions;

namespace DomainProcessors.Extensions.UI.Processors
{
    /// <inheritdoc />
    public class BellazonProcessor : IDomainUIProcessor
    {
        private const String ImageCollectionsSnippet = @"
var links = [];
var images = [];
var posts = document.getElementsByTagName(""article"");
var postInd;
var imageInd;
var link;
for (postInd = 0; postInd < posts.length; ++postInd) {
	images = posts[postInd].getElementsByClassName(""ipsAttachLink ipsAttachLink_image"");
	if(images[0] != undefined) {
		for (imageInd = 0; imageInd < images.length; ++imageInd) {
			link = images[imageInd].href;
			if(link != null) {
				links.push(link);
			}
		}
	}
}
links.toString();";

        public const String DomainName = "www.bellazon.com";
        private const String DownloadTip = "Do not click on any images before download. Please reload the page before attempting download if you did.";
        public String Domain { get; } = DomainName;

        private readonly IJsProcessor _jsProcessor;
        private readonly IProgressHandler _progressHandler;

        public BellazonProcessor(IJsProcessor jsProcessor, IProgressHandler progressHandler)
        {
            _jsProcessor = jsProcessor;
            _progressHandler = progressHandler;
        }

        #region Implementation of IDomainUIProcessor

        /// <inheritdoc />
        public Regex DomainRegex { get; } = new Regex(DomainName);

        /// <inheritdoc />
        public Object WebViewObj { get; set; }


        /// <inheritdoc />
        public async Task<Int32> GetElementsCountAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            return -1;
        }

        /// <inheritdoc />
        public async Task<WorkRequest> ProcessAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //return await _jsProcessor.ProcessAsync(WebViewObj, ImageCollectionsSnippet);
        }

        public Task<WorkRequest> ProcessAsync(IEnumerable<String> urls, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
