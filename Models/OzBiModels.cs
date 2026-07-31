using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OzBiPortalCRM.Models
{
    [Table("tenant")]
    public class OzBiTenant
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsTermsAndConditionsApproved { get; set; }
        public string? TenantTypeCodeId { get; set; }
        public string? RemoteId { get; set; }
        public bool IsSetupCompleted { get; set; }
        public bool IsOnboarded { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }

    [Table("chat")]
    public class OzBiChat
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? ModifiedByUserId { get; set; }
        public string? ExternalContextId { get; set; }
        public string? ExternalIdentifier { get; set; }

        [ForeignKey("TenantId")]
        public virtual OzBiTenant? Tenant { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual OzBiUser? CreatedByUser { get; set; }
    }

    [Table("chatmessage")]
    public class OzBiChatMessage
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Message { get; set; }
        public string? Query { get; set; }
        public int Limit { get; set; }
        public bool HasComplicatedQuery { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Prompt { get; set; }
        public bool IsSucceeded { get; set; }
        public bool IsAsistantMode { get; set; }
        public string? AsistantId { get; set; }
        public string? AIModelId { get; set; }
        public string? ComponentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? Summary { get; set; }
        public string? FeedbackReason { get; set; }
        public bool? IsLiked { get; set; }
        public long? TotalDurationMs { get; set; }
        public long? AIQueryDurationMs { get; set; }
        public long? AIAnalysisDurationMs { get; set; }
        public long? DataFetchDurationMs { get; set; }
        public string? SuggestedQuestions { get; set; }

        [ForeignKey("ChatId")]
        public virtual OzBiChat? Chat { get; set; }

        [ForeignKey("AIModelId")]
        public virtual OzBiAiModel? AIModel { get; set; }

        [ForeignKey("AsistantId")]
        public virtual OzBiAssistant? Assistant { get; set; }
    }

    [Table("aimodel")]
    public class OzBiAiModel
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string AIConnectionSourceCodeId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? ProgrammaticName { get; set; }
        public string? SubModels { get; set; }
        public string? Tools { get; set; }
        public int TokenLimit { get; set; }
        public bool IsStaticModel { get; set; }
        public bool IsSupportsPrediction { get; set; }
        public bool IsDeprecated { get; set; }
        public bool HasTools { get; set; }
        public bool HasMcpSupport { get; set; }
        public string? TenantId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? CustomParameters { get; set; }
    }

    [Table("asistant")]
    public class OzBiAssistant
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool AreAllUsersHavePermission { get; set; }
        public bool IsPublished { get; set; }
        public bool IsSample { get; set; }
        public int AsistantMode { get; set; }
        public bool IsToolsEnabled { get; set; }
        public bool IsMcpEnabled { get; set; }
        public bool IsContinuousChatEnabled { get; set; }
        public string AIConnectionId { get; set; } = string.Empty;
        public string DataConnectionId { get; set; } = string.Empty;
        public string AIModelId { get; set; } = string.Empty;
        public string? SubModel { get; set; }
        public double? Temperature { get; set; }
        public double? TopP { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? AIModelName { get; set; }

        [ForeignKey("DataConnectionId")]
        public virtual OzBiConnection? DataConnection { get; set; }
    }

    [Table("connection")]
    public class OzBiConnection
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public bool IsSample { get; set; }
        public bool IsSystem { get; set; }
        public int CurrentUsage { get; set; }
        public int TotalUsage { get; set; }
        public string ConnectionSourceCodeId { get; set; } = string.Empty;
        public string ConnectionSourceTypeCodeId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? DateCreated { get; set; }

        [ForeignKey("ConnectionSourceCodeId")]
        public virtual OzBiCode? ConnectionSourceCode { get; set; }
    }

    [Table("code")]
    public class OzBiCode
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? DisplayName_TR { get; set; }
        public string? DisplayName_EN { get; set; }
        public string? ProgrammaticName { get; set; }
        public string CodeTypeId { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        [ForeignKey("CodeTypeId")]
        public virtual OzBiCodeType? CodeType { get; set; }
    }

    [Table("codetype")]
    public class OzBiCodeType
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? DisplayName_TR { get; set; }
        public string? DisplayName_EN { get; set; }
        public string? ProgrammaticName { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("aspnetusers")]
    public class OzBiUser
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string? NameSurname { get; set; }
        public bool IsTenantOwner { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int LoginCount { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
