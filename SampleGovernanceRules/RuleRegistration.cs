using SampleGovernanceRules.Rules;
using UiPath.Studio.Activities.Api;
using UiPath.Studio.Activities.Api.Analyzer;

namespace SampleGovernanceRules
{
    public class RuleRegistration : IRegisterAnalyzerConfiguration
    {
        //Registers the rules with Workflow Analyzer
        public void Initialize(IAnalyzerConfigurationService workflowAnalyzerConfigService)
        {
            workflowAnalyzerConfigService.AddRule(PropertySettingsRule.Get());
            workflowAnalyzerConfigService.AddRule(PackageVersionsRule.Get());
        }
    }
}
