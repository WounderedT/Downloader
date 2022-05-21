using Handlers.Configuration;
using Handlers.Configuration.Strategy;
using Handlers.Models;
using Microsoft.Extensions.Options;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Strategy
{
    internal class StrategyFactory: IStrategyFactory
    {
        private readonly DownloadHandlerConfig _options;
        private Dictionary<StrategyEnum, Func<HttpContentHeaders, Boolean>> _strategyRulesDictionary { get; }

        public StrategyFactory(IOptions<DownloadHandlerConfig> options)
        {
            _options = options.Value;
            //_strategyRulesDictionary = _options.Strategies.ToDictionary(s => s.Key, s => BuildStrategyEntryRules(s.Value));
        }

        /// <inheritdoc/>
        public StrategyEnum GetStrategy(String domain)
        {
            if (String.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException(nameof(domain));
            }
            foreach (var strategy in _options.Strategies)
            {
                if(strategy.Value.Domains.Contains(domain))
                {
                    return strategy.Key;
                }
            }
            return StrategyEnum.Default;
        }

        /// <inheritdoc/>
        public StrategyEnum GetStrategy(HttpContentHeaders headers)
        {
            if(headers == null || !headers.Any())
            {
                throw new ArgumentNullException(nameof(headers));
            }
            foreach(var rule in _strategyRulesDictionary)
            {
                if (rule.Value.Invoke(headers))
                {
                    return rule.Key;
                }
            }
            return StrategyEnum.Default;
        }

        private Func<HttpContentHeaders, Boolean> BuildStrategyEntryRules(StrategyConfig strategyEntry)
        {
            if(strategyEntry == null)
            {
                throw new ArgumentNullException(nameof(strategyEntry));
            }
            if(strategyEntry.Rules == null || !strategyEntry.Rules.Any())
            {
                throw new ArgumentException($"{nameof(StrategyConfig)} must have at least one rule defined.");
            }
            Func<HttpContentHeaders, Boolean> rulesFunc = GetComparisonFuncForRule(strategyEntry.Rules.First());
            for(var index = 1; index < strategyEntry.Rules.Count; index++)
            {
                Func<HttpContentHeaders, Boolean> comparionFunc = GetComparisonFuncForRule(strategyEntry.Rules[index]);
                rulesFunc = strategyEntry.Rules[index].Operator switch
                {
                    ConditionalOperatorEnum.AND => headers => rulesFunc(headers) && comparionFunc(headers),
                    ConditionalOperatorEnum.OR => headers => rulesFunc(headers) || comparionFunc(headers),
                    _ => throw new ArgumentException($"{strategyEntry.Rules[index].Operator} is not a valid {nameof(ConditionalOperatorEnum)} value.")
                };
            }
            return rulesFunc;
        }

        private Func<HttpContentHeaders, Boolean> GetComparisonFuncForRule(StrategyRuleConfig rule)
        {
            return rule.Header switch
            {
                Constants.AllowHeader => HeaderComparerFunctions.GetComparisonFunc_Allow(rule.Comparison, rule.Value.FromJsonString<ICollection<string>>()),
                Constants.ContentDispositionHeader => HeaderComparerFunctions.GetComparisonFunc_ContentDisposition(rule.Comparison, rule.Value.FromJsonString<ContentDispositionHeaderValue>()),
                Constants.ContentEncodingHeader => HeaderComparerFunctions.GetComparisonFunc_ContentEncoding(rule.Comparison, rule.Value.FromJsonString<ICollection<string>>()),
                Constants.ContentLanguageHeader => HeaderComparerFunctions.GetComparisonFunc_ContentLanguage(rule.Comparison, rule.Value.FromJsonString<ICollection<string>>()),
                Constants.ContentLengthHeader => HeaderComparerFunctions.GetComparisonFunc_ContentLength(rule.Comparison, Int64.Parse(rule.Value)),
                Constants.ContentLocationHeader => HeaderComparerFunctions.GetComparisonFunc_ContentLocation(rule.Comparison, rule.Value.FromJsonString<Uri>()),
                Constants.ContentMD5Header => HeaderComparerFunctions.GetComparisonFunc_ContentMD5(rule.Comparison, rule.Value.FromJsonString<Byte[]>()),
                Constants.ContentRangeHeader => HeaderComparerFunctions.GetComparisonFunc_ContentRange(rule.Comparison, rule.Value.FromJsonString<ContentRangeHeaderValue>()),
                Constants.ContentTypeHeader => HeaderComparerFunctions.GetComparisonFunc_ContentType(rule.Comparison, rule.Value.FromJsonString<MediaTypeHeaderValue>()),
                Constants.ExpiresHeader => HeaderComparerFunctions.GetComparisonFunc_Expires(rule.Comparison, rule.Value.FromJsonString<DateTimeOffset>()),
                Constants.LastModifiedHeader => HeaderComparerFunctions.GetComparisonFunc_LastModified(rule.Comparison, rule.Value.FromJsonString<DateTimeOffset>()),
                _ => throw new ArgumentException($"{rule.Header} is not a valid {typeof(HttpContentHeaders)} header.")
            };
        }
    }
}
