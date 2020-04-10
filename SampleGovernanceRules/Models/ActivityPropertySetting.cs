using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SampleGovernanceRules.Models
{
    internal class ActivityPropertySetting
    {
        public string Activity { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }
        public bool IgnoreCase { get; set; } = true;

        public void SetProperty(string property, string value)
        {
            property = property.ToLowerInvariant();

            switch (property)
            {
                case "property":
                    this.Property = value;
                    break;
                case "activity":
                    this.Activity = value;
                    break;
                case "value":
                    this.Value = value;
                    break;
                case "ignorecase":
                    bool boolValue = true;
                    bool.TryParse(value, out boolValue);
                    this.IgnoreCase = boolValue;
                    break;
            }
        }

        internal bool ActivityTypeMatches(string activityType)
        {
            var activityRegex = Regex.Escape(this.Activity).Replace("\\*", ".*");
            return PropertyMatchesRegex(activityType, this.Activity, activityRegex);
        }

        internal bool PropertyNameMatches(string property)
        {
            var propertyRegex = Regex.Escape(this.Property).Replace("\\*", ".*");
            return PropertyMatchesRegex(property, this.Property, propertyRegex);
        }

        internal bool ValueMatches(string expression)
        {
            var valueRegex = Regex.Escape(this.Value).Replace("\\*", ".*");
            return PropertyMatchesRegex(expression, this.Value, valueRegex);
        }

        private static bool PropertyMatchesRegex(string input, string property, string regex, bool matchCase = false)
        {
            // An empty value does not match
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            // An exact match
            if (property == input)
            {
                return true;
            }

            RegexOptions options = !matchCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            return Regex.IsMatch(input, $"^{regex}$", options);
        }

    }
}
