using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SampleGovernanceRules.Models;
using Semver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UiPath.Studio.Activities.Api.Analyzer.Rules;
using UiPath.Studio.Analyzer.Models;

[assembly: InternalsVisibleTo("SampleGovernanceRules.Tests")]
namespace SampleGovernanceRules.Rules
{
    static internal class PackageVersionsRule
    {
        public const string RuleId = "ORG-USG-002";
        private const string ConfigParameterKey = "configuration";
        private const string AllowPrereleaseKey = "allowprerelease";

        internal static Rule<IProjectModel> Get()
        {
            var rule = new Rule<IProjectModel>(Strings.ORG_USG_002_Name, RuleId, Inspect)
            {
                RecommendationMessage = Strings.ORG_USG_002_Recommendation,
                ErrorLevel = TraceLevel.Error,
                //Must contain "BusinessRule" to appear in StudioX, rules always appear in Studio
                ApplicableScopes = new List<string> { Constants.BusinessRule }
            };
            rule.Parameters.Add(ConfigParameterKey, new Parameter()
            {
                Key = ConfigParameterKey,
                LocalizedDisplayName = Strings.ConfigurationSettingsLabel,
                DefaultValue = string.Empty
            });
            rule.Parameters.Add(AllowPrereleaseKey, new Parameter()
            {
                Key = AllowPrereleaseKey,
                LocalizedDisplayName = Strings.AllowPrereleasePackagesLabel,
                DefaultValue = "True"
            }); 

            return rule;
        }

        private static InspectionResult Inspect(IProjectModel project, Rule ruleInstance)
        {
            InspectionResult result = new InspectionResult();
            Dictionary<string, PackageVersionSetting> packageSettings = GetPackageSettings(ruleInstance);
            bool prereleaseAllowed = GetPrereleaseSetting(ruleInstance);

            foreach (var dependency in project.Dependencies)
            {
                string packageName = dependency.Name;
                string verStr = (string)dependency.GetType().GetProperty("Version").GetValue(dependency, null);
                if (!IsPackageValid(packageName, verStr, packageSettings, prereleaseAllowed))
                {
                    result.Messages.Add(string.Format(Strings.ORG_USG_002_Message, packageName));
                }
            }

            if (result.Messages.Count > 0)
            {
                result.HasErrors = true;
                result.ErrorLevel = TraceLevel.Error;
                result.RecommendationMessage = ruleInstance.RecommendationMessage;
            }

            return result;
        }

        internal static bool IsPackageValid(string name, string version, Dictionary<string, PackageVersionSetting> settings, bool allowPrerelase)
        {
            PackageVersionSetting setting = null;
            bool hasPackageSettings = settings != null && settings.TryGetValue(name.ToLowerInvariant(), out setting);
            SemVersion packageSemVersion = SemVersion.Parse(version);
            bool packageIsPrerelease = !string.IsNullOrEmpty(packageSemVersion.Prerelease);

            if (packageIsPrerelease)
            {
                bool packagePrereleaseAllowed = allowPrerelase;
                bool packageSpecifiesPrerelease = setting != null && setting.TryGetPreleaseValue(out packagePrereleaseAllowed);
                if((packageSpecifiesPrerelease && !packagePrereleaseAllowed) 
                    || (!packageSpecifiesPrerelease && !allowPrerelase))
                {
                    return false;
                }
            }

            if (!hasPackageSettings)
            {
                return true;
            }

            if (setting.TryGetMinSemVersion(out SemVersion minSemVersion)
                && packageSemVersion < minSemVersion)
            {
                return false;
            }
            if (setting.TryGetMaxSemVersion(out SemVersion maxSemVersion)
                && packageSemVersion > maxSemVersion)
            {
                return false;
            }

            return true;
        }

        private static Dictionary<string, PackageVersionSetting> GetPackageSettings(Rule ruleInstance)
        {
            if (ruleInstance.Parameters.TryGetValue(ConfigParameterKey, out var parameter) &&
                !string.IsNullOrEmpty(parameter.Value)
                && TryParseSettingsJson(parameter.Value, out Dictionary<string, PackageVersionSetting> settings))
            {
                return settings;
            }

            return null;
        }

        private static bool GetPrereleaseSetting(Rule ruleInstance)
        {
            if (ruleInstance.Parameters.TryGetValue(AllowPrereleaseKey, out var parameter) &&
                !string.IsNullOrEmpty(parameter.Value)
                && bool.TryParse(parameter.Value, out bool result))
            {
                return result;
            }
            return true;
        }

        internal static bool TryParseSettingsJson(string parameter, out Dictionary<string, PackageVersionSetting> settings)
        {
            bool success = false;
            settings = null;
            try
            {
                var entry = Newtonsoft.Json.JsonConvert.DeserializeObject<PackageVersionSetting[]>(parameter);
                if (entry != null)
                {
                    settings = new Dictionary<string, PackageVersionSetting>();
                    foreach (var e in entry)
                    {
                        settings.Add(e.Name.ToLowerInvariant(), e);
                    }
                    success = true;
                }
            }
            catch (JsonReaderException)
            {

            }

            return success;
        }
    }
}
