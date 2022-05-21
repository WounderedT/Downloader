using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Handlers.Configuration.Retry
{
    public class RetryPolicyConfig : IDictionary<String, RetryPolicyEntryConfig>
    {
        private const String DefaultDomainStr = "*";
        private readonly Dictionary<String, RetryPolicyEntryConfig> _policyByDomain = new Dictionary<String, RetryPolicyEntryConfig>();
        private RetryPolicyEntryConfig _defaultPolicy;

        private RetryPolicyEntryConfig DefaultPolicy
        {
            get
            {
                if (_defaultPolicy == null)
                {
                    _defaultPolicy = _policyByDomain.TryGetValue(DefaultDomainStr, out RetryPolicyEntryConfig? policy) ? policy : RetryPolicyEntryConfig.Default;
                }
                return _defaultPolicy;
            }
        }

        #region IDictionary implementation

        public RetryPolicyEntryConfig this[String key]
        {
            get => _policyByDomain[key];
            set => _policyByDomain[key] = value;
        }

        public ICollection<String> Keys => _policyByDomain.Keys;

        public ICollection<RetryPolicyEntryConfig> Values => _policyByDomain.Values;

        public Int32 Count => _policyByDomain.Count;

        public Boolean IsReadOnly => true;

        public void Add(String key, RetryPolicyEntryConfig value)
        {
            _policyByDomain.Add(key, value);
        }

        public void Add(KeyValuePair<String, RetryPolicyEntryConfig> item)
        {
            _policyByDomain.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _policyByDomain.Clear();
        }

        public Boolean Contains(KeyValuePair<String, RetryPolicyEntryConfig> item)
        {
            return _policyByDomain.Contains(item);
        }

        public Boolean ContainsKey(String key)
        {
            return _policyByDomain.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<String, RetryPolicyEntryConfig>[] array, Int32 arrayIndex)
        {
            foreach (KeyValuePair<String, RetryPolicyEntryConfig> pair in _policyByDomain)
            {
                array[arrayIndex++] = pair;
            }
        }

        public IEnumerator<KeyValuePair<String, RetryPolicyEntryConfig>> GetEnumerator()
        {
            return _policyByDomain.GetEnumerator();
        }

        public Boolean Remove(String key)
        {
            return _policyByDomain.Remove(key);
        }

        public Boolean Remove(KeyValuePair<String, RetryPolicyEntryConfig> item)
        {
            return _policyByDomain.Remove(item.Key);
        }

        public Boolean TryGetValue(String key, [MaybeNullWhen(false)] out RetryPolicyEntryConfig value)
        {
            return _policyByDomain.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public RetryPolicyEntryConfig GetPolicyOrDefault(String domain)
        {
            if (String.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException(nameof(domain));
            }
            if (_policyByDomain.TryGetValue(domain, out RetryPolicyEntryConfig? policy))
            {
                return policy;
            }
            foreach (var key in _policyByDomain.Keys)
            {
                if (key.Equals(DefaultDomainStr))
                {
                    continue;
                }
                if (new Regex(key).IsMatch(domain))
                {
                    return _policyByDomain[key];
                }
            }
            return DefaultPolicy;
        }
    }
}
