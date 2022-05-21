using Handlers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Configuration.Strategy
{
    public class StrategyRuleConfig
    {
        public ConditionalOperatorEnum Operator { get; set; } = ConditionalOperatorEnum.AND;
        public String Header { get; set; }
        public ComparisonEnum Comparison { get; set; }
        public String Value { get; set; }
    }
}
