using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleGovernanceRules.Rules;
using SampleGovernanceRules.Models;

namespace SampleGovernanceRules.Tests
{
    [TestClass]
    public class PropertySettingsRuleTests
    {
        const string SingleSettingsConfig = "Property:Save to Drafts,Activity:*MailX,Value:True";
        static readonly string MultipleSettingsConfig = $"{SingleSettingsConfig};Property:Save after each row,Activity:*ExcelForEachRow,Value:False";

        [TestMethod]
        public void ParseSingleSettingsEntry()
        {
            var results = PropertySettingsRule.ParseSettingsEntry(SingleSettingsConfig);

            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Save to Drafts", results[0].Property);
            Assert.AreEqual("*MailX", results[0].Activity);
            Assert.AreEqual("True", results[0].Value);
        }
        
        [TestMethod]
        public void ParseMultipleSettingsEntries()
        {
            var results = PropertySettingsRule.ParseSettingsEntry(MultipleSettingsConfig);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("Save after each row", results[1].Property);
            Assert.AreEqual("*ExcelForEachRow", results[1].Activity);
            Assert.AreEqual("False", results[1].Value);
        }

        [TestMethod]
        public void ActivityNameMatches()
        {
            var propertySetting = new ActivityPropertySetting();
            propertySetting.SetProperty("Activity", "*MailX");

            Assert.IsTrue(propertySetting.ActivityTypeMatches("SendMailX"));
            Assert.IsTrue(propertySetting.ActivityTypeMatches("UiPath.Activities.Mail.Business.ForwardMailX"));
            Assert.IsFalse(propertySetting.ActivityTypeMatches("OutlookApplicationCard"));
        }
        
        [TestMethod]
        public void PropertyNameMatches()
        {
            var propertySetting = new ActivityPropertySetting();
            propertySetting.SetProperty("Property", "*Save*draft*");

            Assert.IsTrue(propertySetting.PropertyNameMatches("Save as draft"));
            Assert.IsTrue(propertySetting.PropertyNameMatches("Save to Drafts"));
            Assert.IsFalse(propertySetting.PropertyNameMatches("To"));
        }
        
        [TestMethod]
        public void PropertyValueMatches()
        {
            var propertySetting = new ActivityPropertySetting();
            propertySetting.SetProperty("Value", "True");

            Assert.IsTrue(propertySetting.ValueMatches("True"));
            Assert.IsTrue(propertySetting.ValueMatches("true"));
            Assert.IsFalse(propertySetting.ValueMatches("False"));
            
            propertySetting.SetProperty("Value", "S*");
            Assert.IsTrue(propertySetting.ValueMatches("Skip"));
            Assert.IsTrue(propertySetting.ValueMatches("Stop"));
            Assert.IsFalse(propertySetting.ValueMatches("Continue"));
        }
    }
}
