using DomainProcessors.Extensions.UI.Interfaces;
using Shared.Models.Work;
using System.Text.RegularExpressions;

namespace DomainProcessors.Extensions.UI.Processors
{
    public class ErosberryProcessor : IDomainUIProcessor
    {
        public const String DomainName = "www.erosberry.com";
        public String Domain { get; } = DomainName;

        private const String ImageCollectionsSnippet = @"
var links = [];
var collection = document.getElementsByClassName(""col-2"")[0].getElementsByClassName(""block-post"")[0].getElementsByClassName(""wrap_image"");
var index;
for (index = 0; index < collection.length; ++index) {
		links.push(collection[index].getElementsByTagName(""img"")[0].src);
	}
links.toString();
";

        private const String ErosberryImagePattern =
            "(https?://[a-zA-Z0-9]*.erosberry.com/[a-zA-Z0-9\\.%_~-]*/[a-zA-Z0-9\\.%_~-]*/)[a-zA-Z0-9]*_([0-9]*\\.[a-zA-Z]{1}([a-zA-Z]{1})?([a-zA-Z]{1})?([a-zA-Z]{1})?)";

        private readonly Regex _erosberryImageRegex;
        private readonly IJsProcessor _jsProcessor;

        public ErosberryProcessor(IJsProcessor jsProcessor)
        {
            _erosberryImageRegex = new Regex(ErosberryImagePattern);
            _jsProcessor = jsProcessor;
        }

        #region Implementation of IDomainUIProcessor

        /// <inheritdoc />
        public Regex DomainRegex { get; } = new Regex(DomainName);

        /// <inheritdoc />
        public Object WebViewObj { get; set; }

        ///<inheritdoc />
        public async Task<Int32> GetElementsCountAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            return (await _jsProcessor.ProcessAsync(WebViewObj, ImageCollectionsSnippet)).Count;
        }

        /// <inheritdoc />
        public async Task<WorkRequest> ProcessAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //var result = new ProcessResult();
            //var snippetResult = await webView.InvokeScriptAsync("eval", new[] { ImageCollectionsSnippet });
            //var imagePreviewLinks = snippetResult.Split(ScriptResultSeparator, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var imagePreview in imagePreviewLinks)
            //{
            //    var match = _erosberryImageRegex.Match(imagePreview);
            //    if (!match.Success)
            //    {
            //        throw new ArgumentException($"Image preview [{imagePreview}] does not match expected pattern [{ErosberryImagePattern}]");
            //    }
            //    result.Add($"{match.Groups[1].Value}/{match.Groups[2].Value}");
            //}

            //return result;
        }

        public Task<WorkRequest> ProcessAsync(IEnumerable<String> urls, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
