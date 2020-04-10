using Semver;

namespace SampleGovernanceRules.Models
{
    internal class PackageVersionSetting
    {
        public string Name { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }

        public string AllowPrerelease { get; set; } = null;


        public bool TryGetMaxSemVersion(out SemVersion semVersion)
        {
            return TryParseSemVersion(this.Max, out semVersion);
        }

        public bool TryGetMinSemVersion(out SemVersion semVersion)
        {
            return TryParseSemVersion(this.Min, out semVersion);
        }
        

        private static bool TryParseSemVersion(string version, out SemVersion semVersion)
        {
            if (!string.IsNullOrEmpty(version) && SemVersion.TryParse(version, out semVersion))
            {
                return true;
            }

            semVersion = null;
            return false;
        }

        public bool TryGetPreleaseValue(out bool value)
        {
            return bool.TryParse(this.AllowPrerelease, out value);
        }


    }
}
