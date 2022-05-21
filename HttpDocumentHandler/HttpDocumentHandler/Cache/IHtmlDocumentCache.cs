using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpDocumentHandler.Cache
{
    internal interface IHtmlDocumentCache
    {
        void Add(String url, IHtmlDocument document);

        Boolean TryGetDocument(String url, out IHtmlDocument? document);
    }
}
