using Handlers.Configuration.Retry;
using Handlers.Configuration.Strategy;
using Handlers.Models;

namespace Handlers.Configuration
{
    public class DownloadHandlerConfig
    {
        public Boolean Enabled { get; set; }
        public Boolean RetryOnFailure { get; set; }

        public Boolean SkipMissing { get; set; }
        public Boolean StopOnError { get; set; }

        public String DownloadExtension { get; set; } = ".download";
        public RetryPolicyConfig RetryPolicyByDomain { get; set; } = new RetryPolicyConfig();
        public Dictionary<StrategyEnum, StrategyConfig> Strategies { get; set; } = new Dictionary<StrategyEnum, StrategyConfig>();
    }
}
