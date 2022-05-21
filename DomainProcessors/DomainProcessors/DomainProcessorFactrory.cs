using DomainProcessors.Abstractions;
using DomainProcessors.Configuration;
using Microsoft.Extensions.Options;

namespace DomainProcessors
{
    internal class Constants
    {
        public const string DefaultProcessor = "Default";
    }

    internal class DomainProcessorFactrory : IDomainProcessorFactory
    {
        private readonly List<IDomainProcessor> _domainProcessorsList = new List<IDomainProcessor>();
        private readonly Dictionary<String, IDomainProcessor> _domainProcessors = new Dictionary<String, IDomainProcessor>();

        public DomainProcessorFactrory(IEnumerable<IDomainProcessor> domainProcessors)
        {
            _domainProcessors = domainProcessors.ToDictionary(p => p.Domain, p => p);
            _domainProcessorsList.AddRange(domainProcessors.Where(p => p.DomainRegex != null));
        }

        public IDomainProcessor GetDomainProcessor(String url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            if(_domainProcessors.TryGetValue(url, out IDomainProcessor? domainProcessor))
            {
                return domainProcessor;
            }
            domainProcessor = _domainProcessorsList.SingleOrDefault(prc => prc.DomainRegex.IsMatch(url));
            if(domainProcessor != default)
            {
                return domainProcessor;
            }
            if(_domainProcessors.TryGetValue(Constants.DefaultProcessor, out domainProcessor))
            {
                return domainProcessor;
            }
            throw new NotImplementedException("No default domain processor is registered!");
        }
    }
}
