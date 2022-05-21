using DomainProcessors.Extensions.UI.Interfaces;
using Shared.Models.Work;

namespace DomainProcessors.Extensions.UI
{
    /// <inheritdoc />
    public class JsProcessor : IJsProcessor
    {
        public const Char ScriptResultSeparator = ',';

        #region Implementation of IJsProcessor

        /// <inheritdoc />
        public async Task<WorkSet> ProcessAsync(Object webView, String jsCodeSnippet)
        {
            throw new NotImplementedException();
            //var result = await webView.InvokeScriptAsync("eval", new[] { jsCodeSnippet });
            //return new WorkSet(result.Split(ScriptResultSeparator, StringSplitOptions.RemoveEmptyEntries));
        }

        #endregion
    }
}
