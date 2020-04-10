using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiPath.Studio.Analyzer.Models;

namespace SampleGovernanceRules.Extensions
{
    internal static class ActivityExtensions
    {
        public static string GetActivityId(this IActivityModel activityModel)
        {
            return activityModel.Properties.FirstOrDefault(p => p.DisplayName == "Id")?.DefinedExpression;
        }
    }
}
