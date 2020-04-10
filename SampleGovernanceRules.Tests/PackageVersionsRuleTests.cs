using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleGovernanceRules.Models;
using SampleGovernanceRules.Rules;

namespace SampleGovernanceRules.Tests
{
    [TestClass]
    public class PackageVersionsRuleTests
    {
        const string ComplextScenariosPackage = "UiPath.ComplexScenarios.Activities";
        const string ExcelPackage = "UiPath.Excel.Activities";
        const string UiAutomationNextPackage = "UiPath.UIAutomationNext.Activities";
        const string MailActivitiesPackage = "UiPath.Mail.Activities";

        static readonly string SettingsJson = "[" +
            $"{{Name:\"{UiAutomationNextPackage}\", Min:\"20.4.0-beta.853134\"}}," +
            $"{{Name:\"{ExcelPackage}\", Min:\"1.4.2\", Max:\"1.4.2\"}}," +
            "{Name:\"UiPath.Word.Activities\", Min:\"1.4.2\"}," +
            $"{{Name:\"{ComplextScenariosPackage}\", Min:\"1.0.0\", AllowPrerelease:\"True\"}}," +
            $"{{Name:\"{MailActivitiesPackage}\", Min:\"1.7\", Max:\"1.7.999\", AllowPrerelease:\"False\"}}" +
            "]";

        bool _settingsParsed;
        Dictionary<string, PackageVersionSetting> _settings;

        public PackageVersionsRuleTests()
        {
            _settingsParsed = PackageVersionsRule.TryParseSettingsJson(SettingsJson, out _settings);
        }

        [TestMethod]
        public void AboveMaxVersion()
        {
            bool aboveMaxVersion = PackageVersionsRule.IsPackageValid(ExcelPackage, "1.5.0", _settings, true);

            Assert.IsFalse(aboveMaxVersion);
        }
        
        [TestMethod]
        public void BelowMinVersion()
        {
            bool belowMinVersion = PackageVersionsRule.IsPackageValid(UiAutomationNextPackage, "20.3.0", _settings, true);

            Assert.IsFalse(belowMinVersion);
        }
        
        [TestMethod]
        public void ExactVersion()
        {
            bool exactVersion = PackageVersionsRule.IsPackageValid(ExcelPackage, "1.4.2", _settings, true);

            Assert.IsTrue(exactVersion);
        }

        [TestMethod]
        public void ParsingStatus()
        {
            Assert.IsTrue(_settingsParsed);
            Assert.IsTrue(_settings.ContainsKey("UiPath.UIAutomationNext.Activities".ToLowerInvariant()));
            Assert.IsFalse(_settings.ContainsKey("UiPath.DoesNoteExist"));
        }

        [TestMethod]
        public void InvalidConfig()
        {
            const string badSettingsJson = "Name:\"UiPath.UIAutomation.Next.Activities\", Min:\"20.4.0\"";
            bool success = PackageVersionsRule.TryParseSettingsJson(badSettingsJson, out Dictionary<string, PackageVersionSetting> settings);

            Assert.IsFalse(success);
            Assert.IsNull(settings);
        }

        [TestMethod]
        public void PrereleasePackages()
        {
            bool globallyPermitted = PackageVersionsRule.IsPackageValid(UiAutomationNextPackage, "20.4.0-beta.853134", _settings, true);
            bool individuallyPermitted = PackageVersionsRule.IsPackageValid(ComplextScenariosPackage, "1.0.2-beta.852560", _settings, false);

            Assert.IsTrue(globallyPermitted);
            Assert.IsTrue(individuallyPermitted);
        }

        [TestMethod]
        public void NoPrereleasePackage()
        {
            bool nonPrereleaseVersion = PackageVersionsRule.IsPackageValid(UiAutomationNextPackage, "20.4.0", _settings, true);
            bool prereleaseVersion = PackageVersionsRule.IsPackageValid(UiAutomationNextPackage, "20.3.0", _settings, false);
            bool individuallyBlocked = PackageVersionsRule.IsPackageValid(MailActivitiesPackage, "1.7.5-beta123", _settings, true);
            bool globallyBlocked = PackageVersionsRule.IsPackageValid("UiPath.System.Activities", "20.4.0-beta.864462", _settings, false);

            Assert.IsTrue(nonPrereleaseVersion);
            Assert.IsFalse(prereleaseVersion);
            Assert.IsFalse(individuallyBlocked);
            Assert.IsFalse(globallyBlocked);
        }

        [TestMethod]
        public void RangeTest()
        {
            bool below = PackageVersionsRule.IsPackageValid(MailActivitiesPackage, "1.6.0-beta123", _settings, true);
            bool within = PackageVersionsRule.IsPackageValid(MailActivitiesPackage, "1.7.9", _settings, true);
            bool above = PackageVersionsRule.IsPackageValid(MailActivitiesPackage, "1.6.9", _settings, true);

            Assert.IsFalse(below);
            Assert.IsTrue(within);
            Assert.IsFalse(above);
        }

        [TestMethod]
        public void UnspecifiedPackage()
        {
            Assert.IsTrue(PackageVersionsRule.IsPackageValid("UiPath.System.Activities", "20.4", _settings, true));
        }

    }
}
