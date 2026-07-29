using System;
using System.Collections.Generic;

namespace OzBiPortalCRM.Services
{
    public class MikroComplianceReport
    {
        public int Score { get; set; } = 100;
        public string Grade { get; set; } = "A+"; // A+, A, B, C, F
        public string GradeLabel { get; set; } = "Mükemmel";
        public string SummaryText { get; set; } = string.Empty;
        public bool IsMikroQuery { get; set; } = false;
        public List<MikroRuleCheck> PassedChecks { get; set; } = new();
        public List<MikroRuleViolation> Violations { get; set; } = new();
        public string ProposedTsqlFix { get; set; } = string.Empty;
    }

    public class MikroRuleCheck
    {
        public string RuleId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class MikroRuleViolation
    {
        public string RuleId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int PenaltyPoints { get; set; }
        public string IssueDescription { get; set; } = string.Empty;
        public string V26RuleReference { get; set; } = string.Empty;
        public string RecommendedFix { get; set; } = string.Empty;
    }

    public interface IMikroAuditEngine
    {
        MikroComplianceReport EvaluateQuery(string tsqlQuery, string? userPrompt = null, string? tenantName = null);
    }
}
