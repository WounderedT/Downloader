using DomainProcessors.Extensions.UI.Interfaces;
using Shared.Models.Work;
using System.Text.RegularExpressions;

namespace DomainProcessors.Extensions.UI.Processors
{
    public class FapopediaProcessor : IDomainUIProcessor
    {
        public const String DomainName = "fapopedia.net";
        public String Domain { get; } = DomainName;

        private const Char SnippetResultSeparator = ';';
        private const String GetImagesSnippet = @"
var links = [];
var collection = document.getElementsByClassName(""shrt-blk"")[0].getElementsByTagName(""a"");
var index;
for (index = 0; index < collection.length; ++index) {
	if(collection[index].getElementsByTagName(""img"")[0] != undefined){
            links.push("""".concat(collection[index].href, "";"" , collection[index].getElementsByTagName(""img"")[0].src));
	}
}
links.toString();
";

        //private readonly FapopediaImgHost _imageHost;
        private readonly IJsProcessor _jsProcessor;

        public FapopediaProcessor(IJsProcessor jsProcessor)
        {
            //_imageHost = new FapopediaImgHost();
            _jsProcessor = jsProcessor;
        }

        #region Implementation of IDomainUIProcessor

        /// <inheritdoc />
        public Regex DomainRegex { get; } = new Regex(DomainName);

        /// <inheritdoc />
        public Object WebViewObj { get; set; }

        /// <inheritdoc />
        public async Task<Int32> GetElementsCountAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            return (await GetImagesInternal(WebViewObj)).Length;
        }

        /// <inheritdoc />
        public async Task<WorkRequest> ProcessAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //var result = new ProcessResult();
            //var imagesPerPost = await GetImagesInternal(webView);
            //String[] tmp;
            //foreach (var image in imagesPerPost)
            //{
            //    tmp = image.Split(SnippetResultSeparator);
            //    result.Add(_imageHost.BuildImageUrl(tmp[1]));
            //}

            //return result;
        }

        public Task<WorkRequest> ProcessAsync(IEnumerable<String> urls, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion

        private async Task<String[]> GetImagesInternal(Object webView)
        {
            throw new NotImplementedException();
            //var result = await webView.InvokeScriptAsync("eval", new[] { GetImagesSnippet });
            //return result.Split(ScriptResultSeparator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
