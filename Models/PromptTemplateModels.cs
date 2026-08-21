using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OzBiPortalCRM.Models
{
    public enum PromptCategoryType
    {
        All = 0,
        FinanceAndCashFlow = 1,     // Finans & Nakit Akışı
        SalesAndRevenue = 2,        // Satış & Ciro Analizi
        CustomerAndReceivables = 3, // Müşteriler, Cari & Tahsilat
        StockAndInventory = 4,      // Stok, Depo & Envanter
        OrdersAndLogistics = 5,     // Sipariş & Sevkiyat
        ExecutiveSummaries = 6      // Yönetim & Stratejik Karar
    }

    public enum QuestionOriginType
    {
        All = 0,
        RealTenantAsked = 1,        // Canlıda Sorulmuş (Tenant Ayak İzi)
        StrategicRecommendation = 2 // Sorulabilecek (Stratejik Öneri)
    }

    public class PromptTemplate
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public PromptCategoryType Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = "bi-lightbulb";
        public QuestionOriginType OriginType { get; set; } = QuestionOriginType.StrategicRecommendation;
        public string OriginLabel { get; set; } = "Stratejik Öneri";
        public string TargetRole { get; set; } = "Yönetici"; // "Genel Müdür / CEO", "CFO / Finans Direktörü", "Satış Direktörü", "Satınalma & Depo Müdürü"
        public string Complexity { get; set; } = "Orta";     // "Temel", "Orta", "İleri Düzey"
        public string BusinessImpact { get; set; } = string.Empty; // Bu soru işletmeye ne kazandırır?
        public string ExpectedDecision { get; set; } = string.Empty; // Hangi kararı aldırır?
        public string ErpCompatibility { get; set; } = "Logo v8.0 & Mikro v27.2 Uyumlu";
        
        // Alternatif Sorma Şekilleri (Kullanıcı aynı konuyu nasıl farklı sorabilir?)
        public List<string> AlternativePhrasings { get; set; } = new();

        // Production Stats & Tags
        public int ProductionUsageCount { get; set; }
        public bool IsCustom { get; set; }
        public bool IsFavorited { get; set; }
        public string? HighlightBadge { get; set; } // "🔥 En Çok Sorulan", "⭐ Yönetici Favorisi", "⚠️ Risk Tespiti", "💡 Yeni Fırsat"
        public List<string> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TemplateCategoryInfo
    {
        public PromptCategoryType Category { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = "bi-folder";
        public string BadgeClass { get; set; } = "badge-info";
        public string Description { get; set; } = string.Empty;
        public int TemplateCount { get; set; }
    }

    [Table("CustomPromptTemplates")]
    public class CustomPromptTemplateItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int OriginTypeId { get; set; } = 2;
        public string TargetRole { get; set; } = "Yönetici";
        public string Complexity { get; set; } = "Orta";
        public string BusinessImpact { get; set; } = string.Empty;
        public string ExpectedDecision { get; set; } = string.Empty;
        public string ErpCompatibility { get; set; } = "Logo & Mikro Uyumlu";
        public string? AlternativePhrasingsJson { get; set; }
        public int CreatedByPortalUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
