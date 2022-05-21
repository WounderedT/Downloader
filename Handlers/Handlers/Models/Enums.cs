using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StrategyEnum
    {
        Default,
        Fragmented
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ComparisonEnum
    {
        LessThan,
        LessThanOrEqualTo,
        EqualsTo,
        GreaterThan,
        GreaterThanOrEqualTo
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConditionalOperatorEnum
    {
        AND,
        OR,
    }
}
