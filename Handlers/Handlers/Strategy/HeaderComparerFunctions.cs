using Handlers.Configuration.Strategy;
using Handlers.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Strategy
{
    internal static class HeaderComparerFunctions
    {
        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_Allow(ComparisonEnum comparison, ICollection<string>? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.EqualsTo => value => CompareICollection(value.Allow, expectedValue),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.Allow)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_ContentDisposition(ComparisonEnum comparison, ContentDispositionHeaderValue? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.EqualsTo => value => value.ContentDisposition == null ? expectedValue == null : value.ContentDisposition.Equals(expectedValue),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.ContentDisposition)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_ContentEncoding(ComparisonEnum comparison, ICollection<string>? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.EqualsTo => value => CompareICollection(value.ContentEncoding, expectedValue),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.ContentEncoding)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_ContentLanguage(ComparisonEnum comparison, ICollection<string>? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.EqualsTo => value => CompareICollection(value.ContentLanguage, expectedValue),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.ContentLanguage)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_ContentLength(ComparisonEnum comparison, Int64? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.LessThan => value => (value.ContentLength == null || expectedValue == null) ? false : value.ContentLength.Value < expectedValue.Value,
                ComparisonEnum.LessThanOrEqualTo => value => (value.ContentLength == null || expectedValue == null) ? (value.ContentLength == null && expectedValue == null) : value.ContentLength.Value <= expectedValue.Value,
                ComparisonEnum.EqualsTo => value => (value.ContentLength == null || expectedValue == null) ? (value.ContentLength == null && expectedValue == null) : value.ContentLength.Value == expectedValue.Value,
                ComparisonEnum.GreaterThan => value => (value.ContentLength == null || expectedValue == null) ? false : value.ContentLength.Value > expectedValue.Value,
                ComparisonEnum.GreaterThanOrEqualTo => value => (value.ContentLength == null || expectedValue == null) ? (value.ContentLength == null && expectedValue == null) : value.ContentLength.Value >= expectedValue.Value,
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.ContentLength)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_ContentLocation(ComparisonEnum comparison, Uri? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.EqualsTo => value => value.ContentLocation == null ? expectedValue == null : value.ContentLocation.Equals(expectedValue),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.ContentLocation)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_ContentMD5(ComparisonEnum comparison, Byte[]? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.EqualsTo => value => CompareByteArray(value.ContentMD5, expectedValue),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.ContentMD5)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_ContentRange(ComparisonEnum comparison, ContentRangeHeaderValue? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.EqualsTo => value => value.ContentRange == null ? expectedValue == null : value.ContentRange.Equals(expectedValue),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.ContentRange)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_ContentType(ComparisonEnum comparison, MediaTypeHeaderValue? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.EqualsTo => value => value.ContentType == null ? expectedValue == null : value.ContentType.Equals(expectedValue),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.ContentType)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_Expires(ComparisonEnum comparison, DateTimeOffset? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.LessThan => value => value.Expires.GetValueOrDefault() < expectedValue.GetValueOrDefault(),
                ComparisonEnum.LessThanOrEqualTo => value => value.Expires.GetValueOrDefault() <= expectedValue.GetValueOrDefault(),
                ComparisonEnum.EqualsTo => value => value.Expires.GetValueOrDefault() == expectedValue.GetValueOrDefault(),
                ComparisonEnum.GreaterThan => value => value.Expires.GetValueOrDefault() < expectedValue.GetValueOrDefault(),
                ComparisonEnum.GreaterThanOrEqualTo => value => value.Expires.GetValueOrDefault() >= expectedValue.GetValueOrDefault(),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.Expires)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        public static Func<HttpContentHeaders, Boolean> GetComparisonFunc_LastModified(ComparisonEnum comparison, DateTimeOffset? expectedValue)
        {
            return comparison switch
            {
                ComparisonEnum.LessThan => value => value.LastModified.GetValueOrDefault() < expectedValue.GetValueOrDefault(),
                ComparisonEnum.LessThanOrEqualTo => value => value.LastModified.GetValueOrDefault() <= expectedValue.GetValueOrDefault(),
                ComparisonEnum.EqualsTo => value => value.LastModified.GetValueOrDefault() == expectedValue.GetValueOrDefault(),
                ComparisonEnum.GreaterThan => value => value.LastModified.GetValueOrDefault() < expectedValue.GetValueOrDefault(),
                ComparisonEnum.GreaterThanOrEqualTo => value => value.LastModified.GetValueOrDefault() >= expectedValue.GetValueOrDefault(),
                _ => throw new ArgumentException($"[{comparison}] comparison type is not allowed for the {nameof(HttpContentHeaders.LastModified)} {nameof(StrategyRuleConfig.Header)}")
            };
        }

        private static Boolean CompareICollection(ICollection<String>? left, ICollection<String>? right)
        {
            if(left == null)
            {
                return right == null;
            }
            if(right == null)
            {
                return false;
            }
            if(left.Count != right.Count)
            {
                return false;
            }
            using(var enumeratorLeft = left.GetEnumerator())
            using(var enumeratorRight = right.GetEnumerator())
            {
                while(enumeratorLeft.MoveNext() && enumeratorRight.MoveNext())
                {
                    if(!String.Equals(enumeratorLeft.Current, enumeratorRight.Current))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static Boolean CompareByteArray(Byte[]? left, Byte[]? right)
        {
            if (left == null)
            {
                return right == null;
            }
            if (right == null)
            {
                return false;
            }
            if (left.Length != right.Length)
            {
                return false;
            }
            for(var index = 0; index < left.Length; index++)
            {
                if (left[index] != right[index])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
