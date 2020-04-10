using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGovernanceRules.Extensions
{
    internal static class StringExtensions
    {
        public static string SubstringBefore(this string value, char delimiter)
        {
            if (value == null)
            {
                return null;
            }

            var delimiterIndex = value.IndexOf(delimiter);

            return delimiterIndex < 0 ? value : value.Substring(0, delimiterIndex);
        }
    }
}
