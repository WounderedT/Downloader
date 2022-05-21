using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpDocumentHandler.Cache
{
    internal class CacheDto
    {
        public String Url { get; }
        public IHtmlDocument Document { get; }
        public DateTime Created { get; }

        public CacheDto(String url, IHtmlDocument document)
        {
            Url = url;
            Document = document;
            Created = DateTime.UtcNow;
        }
    }
}
