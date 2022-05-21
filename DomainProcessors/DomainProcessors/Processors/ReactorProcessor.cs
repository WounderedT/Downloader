using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DomainProcessors.Abstractions;
using DomainProcessors.Abstractions.Exceptions;
using DomainProcessors.Configuration;
using DomainProcessors.Extensions;
using HttpDocumentHandler.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;
using Shared.Models.Work;
using System;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace DomainProcessors.Processors
{
    internal class ReactorProcessor : IDomainProcessor
    {
        #region Constants

        private const String PostUrlPattern = @"/post/[0-9]*";
        private const String TagUrlPattern = @"(https?://[a-z\.]*reactor.cc)/tag/(.*)";
        private const String TagAllUrlPattern = @"(https?://[a-z\.]*reactor.cc)/tag/(.*)/new";
        private const String TagAllPaginationUrlPattern = @"(https?://[a-z\.]*reactor.cc)/tag/(.*)/new/([0-9]*)";
        private const String TagBestUrlPattern = @"(https?://[a-z\.]*reactor.cc)/tag/(.*)/best";
        private const String DomainRegexPattern = @"(https?://[a-z\.]*reactor.cc).*";
        private const String PostIdRegexPattern = @"postContainer([0-9]*)";
        private const String TagPageIdRegexPattern = @"/tag/.*/([0-9]*)";
        private const String PaginationButtonQuerySelector = ".next";
        private const String PostContentQuerySelector = ".post_content";
        private const String CommentContentQuerySelector = ".comment_list_post";
        private const String PostsListQuerySelector = ".postContainer";
        private const String PrettyPhotoLinkClassName = "prettyPhotoLink";
        private const String PrettyPhotoLinkQuerySelector = "." + PrettyPhotoLinkClassName;
        private const String ImageQuerySelector = ".image";
        private const String GifQuerySelector = ".video_gif_holder";
        private const String RefererHeader = "http://joyreactor.cc/";
        private const Int32 PostIdRegexGroupId = 1;
        private const Int32 PageRegexGroupId = 1;
        private const Int32 DomainRegexGroupId = 1;
        private const Int32 TagRegexGroupId = 2;
        private const Char TagSpaceChar = '+';

        #endregion

        private readonly ILogger<ReactorProcessor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHtmlHandler _htmlHandler;
        private readonly ReactorProcessorConfig _options;

        private readonly Regex _postUrlRegex = new Regex(PostUrlPattern);
        private readonly Regex _tagUrlRegex = new Regex(TagUrlPattern);
        private readonly Regex _tagAllUrlRegex = new Regex(TagAllUrlPattern);
        private readonly Regex _tagAllPaginationUrlRegex = new Regex(TagAllPaginationUrlPattern);
        private readonly Regex _tagBestUrlRegex = new Regex(TagBestUrlPattern);
        private readonly Regex _postIdRegex = new Regex(PostIdRegexPattern);
        private readonly Regex _tagPageIdRegex = new Regex(TagPageIdRegexPattern);

        public String Domain { get; } = "joyreactor.cc";
        public Regex DomainRegex { get; } = new Regex(DomainRegexPattern);

        public ReactorProcessor(ILogger<ReactorProcessor> logger, IHtmlHandler htmlHandler, IHttpClientFactory httpClientFactory, IOptions<ReactorProcessorConfig> options)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _htmlHandler = htmlHandler;
            _options = options.Value;
        }

        /// <inheritdoc/>
        public async Task<Int32> GetElementsCountAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            return -1;
        }

        /// <inheritdoc/>
        public async Task<WorkRequest> ProcessAsync(IEnumerable<String> urls, CancellationToken cancellationToken = default)
        {
            try
            {
                var workRequest = new WorkRequest();
                foreach (var url in urls)
                {
                    if (!workRequest.TryAdd(await ProcessUrlAsync(url, cancellationToken)))
                    {
                        _logger.LogWarning($"Duplicate URL [{url}] found. Ignoring...");
                    }
                }
                return workRequest;
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<WorkRequest> ProcessAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = new WorkRequest();
                result.Add(await ProcessUrlAsync(galeryUrl, cancellationToken));
                return result;
            }
            catch (Exception exception)
            {
                HandleException(exception);
                throw;
            }
        }

        private async Task<WorkSet> ProcessUrlAsync(String galeryUrl, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrEmpty(galeryUrl))
            {
                throw new ArgumentNullException(nameof(galeryUrl));
            }

            if (_tagUrlRegex.IsMatch(galeryUrl))
            {
                return await ProcessTagAsync(galeryUrl, cancellationToken);
            }

            if (_postUrlRegex.IsMatch(galeryUrl))
            {
                return await ProcessPostAsync(galeryUrl, cancellationToken);
            }

            throw new ArgumentException($"Unrecognized URL [{galeryUrl}] found");
        }

        private async Task<WorkSet> ProcessTagAsync(String galeryUrl, CancellationToken cancellationToken)
        {
            (galeryUrl, var tag) = ProcessTagUrlString(galeryUrl);
            var workSet = new WorkSet(galeryUrl, tag.DecodeUrlString().Replace(TagSpaceChar, ' '), GetDefaultHttpClient());
            IHtmlDocument document = await _htmlHandler.GetHtmlDocumentAsync(galeryUrl, workSet.HttpClient, cancellationToken);

            IElement? paginationButton = document.QuerySelector(PaginationButtonQuerySelector);
            if (paginationButton != null)
            {
                var lastPageIndex = UInt32.Parse(_tagPageIdRegex.Match(paginationButton.GetAttribute("href")).Groups[PageRegexGroupId].Value);
                _logger.LogInformation($"Tag [{tag.DecodeUrlString()}] has [{lastPageIndex + 1}] pages. Processing...");
                IHtmlDocument tagPageDocument;
                var pageCounter = 1;
                var delay = _options.TagPageProcessingDelayMinSec;
                for (var pageIndex = lastPageIndex; pageIndex > 0; pageIndex--)
                {
                    var pageUrl = galeryUrl + "/" + pageIndex;
                    tagPageDocument = await _htmlHandler.GetHtmlDocumentAsync(pageUrl, workSet.HttpClient, cancellationToken);
                    _logger.LogInformation($"Processing page [{pageCounter++}] out of [{lastPageIndex + 1}]...");
                    await ProcessTagPageDocument(pageUrl, tagPageDocument, workSet, cancellationToken);
                    await Task.Delay((delay < _options.TagPageProcessingDelayMaxSec ? delay++ : _options.TagPageProcessingDelayMaxSec) * 1000);
                }
            }

            return await ProcessTagPageDocument(galeryUrl, document, workSet, cancellationToken);
        }

        private async Task<WorkSet> ProcessTagPageDocument(String url, IHtmlDocument document, WorkSet workSet, CancellationToken cancellationToken)
        {
            IHtmlCollection<IElement> postsContainer = document.QuerySelectorAllOrThrow(PostsListQuerySelector);
            var postUrlBase = DomainRegex.Match(url).Groups[DomainRegexGroupId].Value + "/post/";
            foreach (IElement post in postsContainer)
            {
                var id = post.GetAttribute("id");
                if (String.IsNullOrEmpty(id))
                {
                    throw new IParentNodeProcessingException($"Failed to get post ID from 'postContainer' elemenet [{post}]");
                }
                await ProcessPostAsync(postUrlBase + _postIdRegex.Match(id).Groups[PostIdRegexGroupId].Value, cancellationToken, workSet);
            }

            return workSet;
        }

        private async Task<WorkSet> ProcessPostAsync(String galeryUrl, CancellationToken cancellationToken, WorkSet? workSet = null)
        {
            workSet ??= new WorkSet(galeryUrl, GetDefaultHttpClient());
            IHtmlDocument document = await _htmlHandler.GetHtmlDocumentAsync(galeryUrl, workSet.HttpClient, cancellationToken);

            ProcessSelectorContentAsync(document, workSet, PostContentQuerySelector);
            ProcessSelectorContentAsync(document, workSet, CommentContentQuerySelector);

            return workSet;
        }

        private WorkSet ProcessSelectorContentAsync(IHtmlDocument document, WorkSet workSet, String contentQuerySelector)
{
            IElement? content = document.QuerySelectorOrThrow(contentQuerySelector);

            //Processing pretty photos before images to make sure they overwrite duplicated images.
            ProcessPrettyPhotoLinks(content, workSet);
            ProcessPostImages(content, workSet);
            ////can a post contain both prettyphotos and plain images?
            //if (content.QuerySelector(PrettyPhotoLinkQuerySelector) != null)
            //{
            //    ProcessPrettyPhotoLinks(content, workSet);
            //}
            //else
            //{
            //    ProcessPostImages(content, workSet);
            //}

            return workSet;
        }

        private WorkSet ProcessPrettyPhotoLinks(IElement postContent, WorkSet workSet)
        {
            foreach (IElement element in postContent.QuerySelectorAll(PrettyPhotoLinkQuerySelector))
            {
                var href = element.GetAttribute("href");
                if (href != null)
                {
                    if(!workSet.TryAdd(new UnitOfWork(href)))
                    {
                        _logger.LogDebug($"Trying to add duplicated URL [{href}] to {nameof(WorkSet)}. Ignoring...");
                    }
                }
            }
            return workSet;
        }

        private WorkSet ProcessPostImages(IElement postContent, WorkSet workSet)
        {
            foreach (IElement element in postContent.QuerySelectorAll(ImageQuerySelector))
            {
                var gifElement = element.QuerySelector(GifQuerySelector);
                if (gifElement != null)
                {
                    var href = gifElement.QuerySelector("a")?.GetAttribute("href");
                    if (href != null)
                    {
                        if (!workSet.TryAdd(new UnitOfWork(href)))
                        {
                            _logger.LogDebug($"Trying to add duplicated URL [{href}] to {nameof(WorkSet)}. Ignoring...");
                        }
                    }
                    continue;
                }
                var imageElement = element.QuerySelector("img");
                if(imageElement == null)
                {
                    continue;
                }
                if(String.IsNullOrEmpty(imageElement.ParentElement?.ClassName) || !imageElement.ParentElement.ClassName.Equals(PrettyPhotoLinkClassName))
                {
                    var src = element.QuerySelector("img")?.GetAttribute("src");
                    if (src != null)
                    {
                        if (!workSet.TryAdd(new UnitOfWork(src)))
                        {
                            _logger.LogDebug($"Trying to add duplicated URL [{src}] to {nameof(WorkSet)}. Ignoring...");
                        }
                    }
                }
            }
            return workSet;
        }

        private HttpClient GetDefaultHttpClient()
        {
            HttpClient? httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("Referer", RefererHeader);
            return httpClient;
        }

        private (String url, String tag) ProcessTagUrlString(String url)
        {
            Match? matchBest = _tagBestUrlRegex.Match(url);
            if (matchBest.Success)
            {
                return (url, matchBest.Groups[TagRegexGroupId].Value);
            }
            Match? matchAll = _tagAllUrlRegex.Match(url);
            if (matchAll.Success)
            {
                return (url, matchAll.Groups[TagRegexGroupId].Value);
            }
            return (url + "/new", _tagUrlRegex.Match(url).Groups[TagRegexGroupId].Value);
        }

        private void HandleException(Exception exception)
        {
            _logger.LogError(exception.Message);
            _logger.LogException(exception);
        }
    }
}
