using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OzBiPortalCRM.Models
{
    [Table("TenantComplianceSnapshots")]
    public class TenantComplianceSnapshot
    {
        [Key]
        public string TenantId { get; set; } = string.Empty;

        public string TenantName { get; set; } = string.Empty;

        public string ErpType { get; set; } = "Generic";

        public string ErpTypeName { get; set; } = "Genel ERP";

        public int OverallScore { get; set; } = 100;

        public string Grade { get; set; } = "A+";

        public string GradeLabel { get; set; } = "Mükemmel";

        public int TotalQueriesEvaluated { get; set; }

        public int CompliantCount { get; set; } // Score >= 90 (A/A+)

        public int WarningCount { get; set; } // 60 <= Score < 90 (B/C)

        public int CriticalCount { get; set; } // Score < 60 (D/F)

        public bool IsPromptSynced { get; set; } = true;

        public string PromptVersionLabel { get; set; } = "Güncel";

        public string PromptSyncDetails { get; set; } = string.Empty;

        public string? TopViolationsJson { get; set; }

        public DateTime LastEvaluatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TenantComplianceScorecard
    {
        public string TenantId { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string ErpType { get; set; } = "Generic";
        public string ErpTypeName { get; set; } = "Genel ERP";

        public int OverallScore { get; set; } = 100;
        public string Grade { get; set; } = "A+";
        public string GradeLabel { get; set; } = "Mükemmel";

        public int TotalQueriesEvaluated { get; set; }
        public int CompliantCount { get; set; }
        public int WarningCount { get; set; }
        public int CriticalCount { get; set; }

        public double CompliantPercentage => TotalQueriesEvaluated > 0 ? Math.Round((double)CompliantCount / TotalQueriesEvaluated * 100, 1) : 100.0;
        public double WarningPercentage => TotalQueriesEvaluated > 0 ? Math.Round((double)WarningCount / TotalQueriesEvaluated * 100, 1) : 0.0;
        public double CriticalPercentage => TotalQueriesEvaluated > 0 ? Math.Round((double)CriticalCount / TotalQueriesEvaluated * 100, 1) : 0.0;

        public bool IsPromptSynced { get; set; } = true;
        public string PromptVersionLabel { get; set; } = "Güncel";
        public string PromptSyncDetails { get; set; } = string.Empty;

        public List<TenantRuleViolationStat> TopViolations { get; set; } = new();
        public List<TenantQueryComplianceSummary> EvaluatedQueries { get; set; } = new();

        public DateTime LastEvaluatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TenantRuleViolationStat
    {
        public string RuleId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Severity { get; set; } = "Error"; // "Error", "Warning", "Info"
        public int Count { get; set; }
        public double Percentage { get; set; }
        public int TotalPenaltyPoints { get; set; }
        public string RecommendedFix { get; set; } = string.Empty;
        public string V26RuleReference { get; set; } = string.Empty;
        public bool IsCoveredByGoldenPrompt { get; set; } = false;
    }

    public class TenantQueryComplianceSummary
    {
        public string MessageId { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public string? QuestionText { get; set; }
        public string SqlQuery { get; set; } = string.Empty;
        public int Score { get; set; }
        public string Grade { get; set; } = "A+";
        public string GradeLabel { get; set; } = "Mükemmel";
        public bool IsSucceeded { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public DateTime? DateCreated { get; set; }
        public List<string> ViolationTitles { get; set; } = new();
    }
}
