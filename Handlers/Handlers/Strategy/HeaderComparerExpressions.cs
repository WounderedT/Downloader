using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Handlers.Strategy
{
    internal static class HeaderComparerExpressions
    {
        public static Expression StringEqualityExpression(String expectedValue)
        {
            Expression<Func<String,Boolean>> expression = (actualStr) => String.IsNullOrEmpty(actualStr) ? String.IsNullOrEmpty(expectedValue) : String.Equals(expectedValue, actualStr);
            return expression;
        }
    }
}
