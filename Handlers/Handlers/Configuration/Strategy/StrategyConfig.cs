using Handlers.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Configuration.Strategy
{
    public class StrategyConfig
    {
        public List<StrategyRuleConfig> Rules { get; set; } = new List<StrategyRuleConfig>();
        public HashSet<String> Domains { get; set; } = new HashSet<String>();
    }
}
