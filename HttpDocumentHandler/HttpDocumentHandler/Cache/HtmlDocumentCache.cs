using AngleSharp.Html.Dom;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpDocumentHandler.Cache
{
    internal class HtmlDocumentCache: IHtmlDocumentCache
    {
        private readonly ConcurrentDictionary<String, CacheDto> _cache;
        private readonly TimeSpan _ttl;

        public HtmlDocumentCache() : this(TimeSpan.FromSeconds(300)) { }

        public HtmlDocumentCache(TimeSpan ttl)
        {
            _ttl = ttl;
            _cache = new ConcurrentDictionary<String, CacheDto>();
        }

        public void Add(String url, IHtmlDocument document)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            var obj = new CacheDto(url, document);
            _cache.AddOrUpdate(url, obj, (url, dto) => dto);
        }

        public Boolean TryGetDocument(String url, out IHtmlDocument? document)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            document = null;
            if(_cache.TryGetValue(url, out var dto))
            {
                if(DateTime.UtcNow - dto.Created < _ttl)
                {
                    document = dto.Document;
                    return true;
                }
                else
                {
                    _cache.Remove(url, out _);
                }
            }
            return false;
        }
    }
}
