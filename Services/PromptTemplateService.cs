using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OzBiPortalCRM.Data;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public class PromptTemplateService : IPromptTemplateService
    {
        private readonly AppDbContext _appDbContext;
        private static readonly Random _random = new();

        // User-scoped cache for dynamically generated questions (key = portalUserId)
        private static readonly ConcurrentDictionary<int, List<PromptTemplate>> _dynamicCacheByUser = new();

        public PromptTemplateService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<PromptTemplate>> GetAllQuestionsAsync(
            PromptCategoryType category = PromptCategoryType.All,
            QuestionOriginType originType = QuestionOriginType.All,
            string? targetRole = null,
            string? searchTerm = null,
            string? complexity = null,
            int portalUserId = 0)
        {
            var questions = new List<PromptTemplate>();
            var effectiveUserId = portalUserId > 0 ? portalUserId : 1;

            // 1. Add user-scoped dynamically generated questions first
            if (_dynamicCacheByUser.TryGetValue(effectiveUserId, out var userDynCache))
            {
                lock (userDynCache)
                {
                    questions.AddRange(userDynCache);
                }
            }

            // 2. Add curated question catalog
            questions.AddRange(GetCuratedQuestions());

            // 3. Load custom questions added via SQLite (with AlternativePhrasingsJson parsing)
            try
            {
                var customItems = await _appDbContext.CustomPromptTemplates.ToListAsync();
                foreach (var c in customItems)
                {
                    var altPhrasings = new List<string>();
                    if (!string.IsNullOrWhiteSpace(c.AlternativePhrasingsJson))
                    {
                        try
                        {
                            altPhrasings = JsonSerializer.Deserialize<List<string>>(c.AlternativePhrasingsJson) ?? new();
                        }
                        catch { }
                    }

                    questions.Add(new PromptTemplate
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Prompt = c.Prompt,
                        Category = (PromptCategoryType)c.CategoryId,
                        CategoryName = GetCategoryName((PromptCategoryType)c.CategoryId),
                        CategoryIcon = "",
                        OriginType = (QuestionOriginType)c.OriginTypeId,
                        OriginLabel = c.OriginTypeId == 1 ? "Popüler" : "Öneri",
                        TargetRole = c.TargetRole ?? "Yönetici",
                        Complexity = c.Complexity ?? "Orta",
                        BusinessImpact = c.BusinessImpact ?? string.Empty,
                        ExpectedDecision = c.ExpectedDecision ?? string.Empty,
                        ErpCompatibility = c.ErpCompatibility ?? "Logo & Mikro Uyumlu",
                        HighlightBadge = "Özel Soru",
                        IsCustom = true,
                        AlternativePhrasings = altPhrasings,
                        CreatedAt = c.CreatedAt
                    });
                }
            }
            catch { }

            // 4. Load user favorites from local SQLite database
            try
            {
                var favList = await _appDbContext.Favorites
                    .Where(f => f.PortalUserId == effectiveUserId && f.ItemType == "PromptTemplate")
                    .Select(f => f.ItemId)
                    .ToListAsync();
                var favIds = favList.ToHashSet();

                foreach (var q in questions)
                {
                    q.IsFavorited = favIds.Contains(q.Id);
                }
            }
            catch { }

            // Filtreleme artık UI katmanında (PromptTemplates.razor ApplyFilters) yapılıyor.
            // Servis tüm soruları döndürür, böylece GetCategoriesSummaryAsync ile veri paylaşılabilir.

            return questions;
        }

        public async Task<List<PromptTemplate>> GenerateFreshDynamicQuestionsAsync(
            int count = 8, 
            PromptCategoryType category = PromptCategoryType.All, 
            int portalUserId = 0)
        {
            var effectiveUserId = portalUserId > 0 ? portalUserId : 1;
            var targetCategories = category == PromptCategoryType.All
                ? new[] {
                    PromptCategoryType.FinanceAndCashFlow,
                    PromptCategoryType.SalesAndRevenue,
                    PromptCategoryType.CustomerAndReceivables,
                    PromptCategoryType.StockAndInventory,
                    PromptCategoryType.OrdersAndLogistics,
                    PromptCategoryType.ExecutiveSummaries
                }
                : new[] { category };

            var timeframes = new[] { "Son 15 günde", "Son 30 günde", "Son 60 günde", "Bu çeyrekte", "Yılbaşından bu yana", "Son 6 ayda" };
            var thresholds = new[] { "50.000 TL üzeri", "100.000 TL üzeri", "250.000 TL üzeri", "500.000 TL üzeri" };

            var dynamicGenerators = new List<Func<PromptTemplate>>
            {
                // 1. Finans & Banka
                () => {
                    var tf = timeframes[_random.Next(timeframes.Length)];
                    var th = thresholds[_random.Next(thresholds.Length)];
                    return new PromptTemplate
                    {
                        Id = "dyn-fin-" + Guid.NewGuid().ToString("N")[..8],
                        Title = $"Banka POS ve Komisyon Maliyeti Analizi",
                        Prompt = $"{tf} bankalara ödediğimiz POS komisyon oranları ve toplam kesinti tutarlarını banka bazında listele",
                        Category = PromptCategoryType.FinanceAndCashFlow,
                        CategoryName = "Finans & Nakit Akışı",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "CFO / Finans Müdürü",
                        Complexity = "Orta",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            $"Bankaların {tf.ToLowerInvariant()} uyguladığı komisyon kesintilerini karşılaştır",
                            $"En çok POS komisyonu ödediğimiz bankalar hangileri?"
                        },
                        Tags = new List<string> { "POS", "Komisyon", "Banka Maliyeti" }
                    };
                },
                // 2. Finans & Likidite
                () => {
                    var days = new[] { "7 gün", "15 gün", "30 gün", "45 gün" }[_random.Next(4)];
                    return new PromptTemplate
                    {
                        Id = "dyn-fin-" + Guid.NewGuid().ToString("N")[..8],
                        Title = $"Önümüzdeki {days}'lik Net Likidite Dengesi",
                        Prompt = $"Gelecek {days} içinde vadesi gelecek müşteri çekleri ile ödenecek satıcı senetlerinin net nakit farkı nedir?",
                        Category = PromptCategoryType.FinanceAndCashFlow,
                        CategoryName = "Finans & Nakit Akışı",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "Hazine / Finans Direktörü",
                        Complexity = "İleri Düzey",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            $"Önümüzdeki {days} içinde kasaya girecek çekler ve çıkacak ödemeleri karşılaştır"
                        },
                        Tags = new List<string> { "Nakit Farkı", "Çek Senet", "Likidite" }
                    };
                },
                // 3. Satış & Müşteri (Bug4 Fix: OriginType → StrategicRecommendation)
                () => {
                    var tf = timeframes[_random.Next(timeframes.Length)];
                    var topN = new[] { "5", "10", "15", "20" }[_random.Next(4)];
                    return new PromptTemplate
                    {
                        Id = "dyn-sal-" + Guid.NewGuid().ToString("N")[..8],
                        Title = $"{tf} En Çok Satış Yapılan İlk {topN} Ürün Grubu",
                        Prompt = $"{tf} en yüksek satış cirosu üreten ilk {topN} ürün kategorisini ve toplam satış adetlerini getir",
                        Category = PromptCategoryType.SalesAndRevenue,
                        CategoryName = "Satış & Ciro",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "Satış Direktörü",
                        Complexity = "Temel",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            $"{tf} ciromuzun en büyük kısmını oluşturan ilk {topN} ürün grubu hangisidir?"
                        },
                        Tags = new List<string> { "Ürün Grubu", "Ciro", "Satış Adedi" }
                    };
                },
                // 4. Satış & Kârlılık
                () => {
                    var tf = timeframes[_random.Next(timeframes.Length)];
                    return new PromptTemplate
                    {
                        Id = "dyn-sal-" + Guid.NewGuid().ToString("N")[..8],
                        Title = "Bölge Bazlı Satış ve Kâr Dağılımı",
                        Prompt = $"{tf} iller ve satış bölgeleri bazında elde edilen net satış cirosu ve brüt kâr oranlarını karşılaştır",
                        Category = PromptCategoryType.SalesAndRevenue,
                        CategoryName = "Satış & Ciro",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "Genel Müdür / Satış Direktörü",
                        Complexity = "Orta",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            $"Hangi şehirlerde veya bölgelerde en yüksek kâr marjıyla satış yapıyoruz?"
                        },
                        Tags = new List<string> { "Bölge Satışı", "İl Bazında Ciro", "Kârlılık" }
                    };
                },
                // 5. Cari & Risk (Bug4 Fix: OriginType → StrategicRecommendation)
                () => {
                    var delayDays = new[] { "30 günü", "45 günü", "60 günü", "90 günü" }[_random.Next(4)];
                    var th = thresholds[_random.Next(thresholds.Length)];
                    return new PromptTemplate
                    {
                        Id = "dyn-rec-" + Guid.NewGuid().ToString("N")[..8],
                        Title = $"Vadesi {delayDays} Aşan Yüksek Riskli Cariler",
                        Prompt = $"Vadesi {delayDays} aşmış ve toplam borcu {th} olan müşterileri ve yetkili iletişim bilgilerini listele",
                        Category = PromptCategoryType.CustomerAndReceivables,
                        CategoryName = "Müşteriler & Tahsilat",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "Kredi & Tahsilat Müdürü",
                        Complexity = "Orta",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            $"Gecikmesi {delayDays} geçen büyük borçlu müşteri listesi"
                        },
                        Tags = new List<string> { "Vade Aşımı", "Tahsilat", "Risk" }
                    };
                },
                // 6. Cari & Tahsilat Hızı
                () => {
                    var tf = timeframes[_random.Next(timeframes.Length)];
                    return new PromptTemplate
                    {
                        Id = "dyn-rec-" + Guid.NewGuid().ToString("N")[..8],
                        Title = "Ödeme Süresi İyileşen ve Kötüleşen Müşteriler",
                        Prompt = $"{tf} ortalama fatura ödeme süresi (DSO) uzayan ve nakit akışını riske atan müşterileri sırala",
                        Category = PromptCategoryType.CustomerAndReceivables,
                        CategoryName = "Müşteriler & Tahsilat",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "CFO / Risk Komitesi",
                        Complexity = "İleri Düzey",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            $"Ödemelerini eskiye göre daha geç yapmaya başlayan cariler kimler?"
                        },
                        Tags = new List<string> { "DSO", "Ödeme Alışkanlığı", "Risk" }
                    };
                },
                // 7. Stok & Envanter
                () => {
                    var tf = timeframes[_random.Next(timeframes.Length)];
                    return new PromptTemplate
                    {
                        Id = "dyn-stk-" + Guid.NewGuid().ToString("N")[..8],
                        Title = "Son Dönemde Fiyatı En Çok Artan Malzemeler",
                        Prompt = $"{tf} birim satınalma maliyeti en çok artan hammadde ve ticari malları artış yüzdeleriyle göster",
                        Category = PromptCategoryType.StockAndInventory,
                        CategoryName = "Stok & Envanter",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "Satınalma & Maliyet Muhasebesi",
                        Complexity = "Orta",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            $"{tf} maliyeti fırlayan ürünler ve girdi fiyat enflasyonu tablosu"
                        },
                        Tags = new List<string> { "Maliyet Artışı", "Satınalma", "Enflasyon" }
                    };
                },
                // 8. Stok & Ambar (Bug4 Fix: OriginType → StrategicRecommendation)
                () => {
                    return new PromptTemplate
                    {
                        Id = "dyn-stk-" + Guid.NewGuid().ToString("N")[..8],
                        Title = "Emniyet Stoku Sınırındaki Hammaddeler",
                        Prompt = "Üretim için kritik olan ve depodaki mevcut miktarı 1 haftalık tüketim miktarının altına inen hammaddeleri listele",
                        Category = PromptCategoryType.StockAndInventory,
                        CategoryName = "Stok & Envanter",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "Üretim Planlama & Depo",
                        Complexity = "İleri Düzey",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            "1 hafta içinde tükenebilecek kritik hammadde stoklarımız nelerdir?"
                        },
                        Tags = new List<string> { "Emniyet Stoku", "Hammadde", "Üretim" }
                    };
                },
                // 9. Sipariş & Lojistik (Bug4 Fix: OriginType → StrategicRecommendation)
                () => {
                    var tf = timeframes[_random.Next(timeframes.Length)];
                    return new PromptTemplate
                    {
                        Id = "dyn-ord-" + Guid.NewGuid().ToString("N")[..8],
                        Title = "Kısmi Sevk Edilmiş ve Kalan Siparişler",
                        Prompt = $"{tf} bir kısmı sevk edilmiş ancak kalan bakiyesi henüz müşteriye teslim edilmemiş açık siparişleri listele",
                        Category = PromptCategoryType.OrdersAndLogistics,
                        CategoryName = "Sipariş & Sevkiyat",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "Lojistik & Sevkiyat Şefi",
                        Complexity = "Temel",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            "Parça parça teslimatı bekleyen siparişlerin kalan miktarları nedir?"
                        },
                        Tags = new List<string> { "Kısmi Sevkiyat", "Kalan Bakiye", "Sipariş" }
                    };
                },
                // 10. Yönetim & Strateji
                () => {
                    var tf = timeframes[_random.Next(timeframes.Length)];
                    return new PromptTemplate
                    {
                        Id = "dyn-exe-" + Guid.NewGuid().ToString("N")[..8],
                        Title = "Ciro ve Faaliyet Giderleri Oranı (EBITDA Katkısı)",
                        Prompt = $"{tf} toplam satış gelirimizin ne kadarı operasyonel giderlere (personel, nakliye, enerji) harcandı?",
                        Category = PromptCategoryType.ExecutiveSummaries,
                        CategoryName = "Yönetim & Strateji",
                        OriginType = QuestionOriginType.StrategicRecommendation,
                        OriginLabel = "Yeni Üretildi",
                        TargetRole = "Genel Müdür / CFO",
                        Complexity = "İleri Düzey",
                        HighlightBadge = "✨ Yeni Öneri",
                        AlternativePhrasings = new List<string>
                        {
                            $"{tf} net faaliyet kâr marjımız ve giderlerimizin ciroya oranı nedir?"
                        },
                        Tags = new List<string> { "Faaliyet Gideri", "EBITDA", "Genel Müdür" }
                    };
                }
            };

            // Shuffle generators and produce requested count of dynamic questions
            var shuffledGens = dynamicGenerators.OrderBy(_ => _random.Next()).ToList();
            var chosen = shuffledGens.Take(Math.Min(count, shuffledGens.Count)).Select(g => g()).ToList();

            // Bug2 Fix: Clear old generated questions, replace with fresh ones
            var userCache = _dynamicCacheByUser.GetOrAdd(effectiveUserId, _ => new List<PromptTemplate>());
            lock (userCache)
            {
                userCache.Clear();
                userCache.AddRange(chosen);
            }

            return await GetAllQuestionsAsync(portalUserId: portalUserId);
        }

        public async Task<List<TemplateCategoryInfo>> GetCategoriesSummaryAsync()
        {
            var all = await GetAllQuestionsAsync();
            return BuildCategorySummary(all);
        }

        /// <summary>
        /// Önceden yüklenmiş soru listesinden kategori özetini oluşturur.
        /// UI'dan LoadData() çağrısında duplike DB sorgusu yapılmaması için kullanılır.
        /// </summary>
        public List<TemplateCategoryInfo> GetCategoriesSummary(List<PromptTemplate> preloaded)
        {
            return BuildCategorySummary(preloaded);
        }

        private static List<TemplateCategoryInfo> BuildCategorySummary(List<PromptTemplate> all)
        {
            return new List<TemplateCategoryInfo>
            {
                new() { Category = PromptCategoryType.All, Name = "Tüm Sorular", Icon = "", BadgeClass = "badge-primary", Description = "Tüm işlevsel alanlardaki doğrulanmış sorular", TemplateCount = all.Count },
                new() { Category = PromptCategoryType.FinanceAndCashFlow, Name = "Finans & Nakit Akışı", Icon = "", BadgeClass = "badge-success", Description = "Kasa, banka, kredi, faiz ve likidite soruları", TemplateCount = all.Count(q => q.Category == PromptCategoryType.FinanceAndCashFlow) },
                new() { Category = PromptCategoryType.SalesAndRevenue, Name = "Satış & Ciro", Icon = "", BadgeClass = "badge-info", Description = "Faturalar, iadeler, ciro kıyaslamaları ve kârlılık", TemplateCount = all.Count(q => q.Category == PromptCategoryType.SalesAndRevenue) },
                new() { Category = PromptCategoryType.CustomerAndReceivables, Name = "Müşteriler & Tahsilat", Icon = "", BadgeClass = "badge-warning", Description = "Vadesi geçmiş alacaklar, yaşlandırma ve çek riski", TemplateCount = all.Count(q => q.Category == PromptCategoryType.CustomerAndReceivables) },
                new() { Category = PromptCategoryType.StockAndInventory, Name = "Stok & Envanter", Icon = "", BadgeClass = "badge-danger", Description = "Kritik stoklar, depo bakiyeleri ve atıl envanter", TemplateCount = all.Count(q => q.Category == PromptCategoryType.StockAndInventory) },
                new() { Category = PromptCategoryType.OrdersAndLogistics, Name = "Sipariş & Sevkiyat", Icon = "", BadgeClass = "badge-secondary", Description = "Açık siparişler, gecikmeler ve karşılama oranları", TemplateCount = all.Count(q => q.Category == PromptCategoryType.OrdersAndLogistics) },
                new() { Category = PromptCategoryType.ExecutiveSummaries, Name = "Yönetim & Strateji", Icon = "", BadgeClass = "badge-purple", Description = "C-Level KPI panoları, çeyreklik büyüme ve bütçe", TemplateCount = all.Count(q => q.Category == PromptCategoryType.ExecutiveSummaries) }
            };
        }

        public async Task<PromptTemplate?> GetQuestionByIdAsync(string id, int portalUserId = 0)
        {
            var all = await GetAllQuestionsAsync(portalUserId: portalUserId);
            return all.FirstOrDefault(q => q.Id == id);
        }

        public async Task<bool> AddCustomQuestionAsync(CustomPromptTemplateItem item)
        {
            try
            {
                _appDbContext.CustomPromptTemplates.Add(item);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCustomQuestionAsync(string id, int portalUserId)
        {
            try
            {
                var item = await _appDbContext.CustomPromptTemplates.FindAsync(id);
                if (item != null)
                {
                    _appDbContext.CustomPromptTemplates.Remove(item);
                    await _appDbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Dictionary<string, int>> GetQuestionStatsAsync()
        {
            var all = await GetAllQuestionsAsync();
            return BuildStats(all);
        }

        private static Dictionary<string, int> BuildStats(List<PromptTemplate> all)
        {
            return new Dictionary<string, int>
            {
                ["Total"] = all.Count,
                ["RealTenantAsked"] = all.Count(q => q.OriginType == QuestionOriginType.RealTenantAsked),
                ["StrategicRecommendation"] = all.Count(q => q.OriginType == QuestionOriginType.StrategicRecommendation),
                ["Finance"] = all.Count(q => q.Category == PromptCategoryType.FinanceAndCashFlow),
                ["Sales"] = all.Count(q => q.Category == PromptCategoryType.SalesAndRevenue),
                ["Receivables"] = all.Count(q => q.Category == PromptCategoryType.CustomerAndReceivables),
                ["Stock"] = all.Count(q => q.Category == PromptCategoryType.StockAndInventory),
                ["Orders"] = all.Count(q => q.Category == PromptCategoryType.OrdersAndLogistics),
                ["Executive"] = all.Count(q => q.Category == PromptCategoryType.ExecutiveSummaries)
            };
        }

        private static string GetCategoryName(PromptCategoryType cat) => cat switch
        {
            PromptCategoryType.FinanceAndCashFlow => "Finans & Nakit Akışı",
            PromptCategoryType.SalesAndRevenue => "Satış & Ciro",
            PromptCategoryType.CustomerAndReceivables => "Müşteriler & Tahsilat",
            PromptCategoryType.StockAndInventory => "Stok & Envanter",
            PromptCategoryType.OrdersAndLogistics => "Sipariş & Sevkiyat",
            PromptCategoryType.ExecutiveSummaries => "Yönetim & Strateji",
            _ => "Genel Sorular"
        };

        private static List<PromptTemplate> GetCuratedQuestions()
        {
            return new List<PromptTemplate>
            {
                // =========================================================================
                // 1. FİNANS & NAKİT AKIŞI
                // =========================================================================
                new()
                {
                    Id = "q-fin-01",
                    Title = "Kasa Nakit Akış Dengesi",
                    Prompt = "Son 30 günde kasaya giren ve kasadan çıkan nakit hareketlerini ve net nakit durumunu özetle",
                    Category = PromptCategoryType.FinanceAndCashFlow,
                    CategoryName = "Finans & Nakit Akışı",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Finans & Muhasebe Müdürü",
                    Complexity = "Temel",
                    AlternativePhrasings = new List<string>
                    {
                        "Kasalarımızın son 1 aylık giriş çıkış toplamlarını listele",
                        "Nakit kasamızda son 30 günde ne kadar para toplandı ve nereye harcandı?",
                        "Kasa bakiyelerini ve aylık nakit akış dengesini göster"
                    },
                    ProductionUsageCount = 142,
                    Tags = new List<string> { "Kasa", "Nakit Akışı", "Likidite", "Nakit Giriş" }
                },
                new()
                {
                    Id = "q-fin-02",
                    Title = "Kredi Faiz ve Ana Para Dökümü",
                    Prompt = "Banka kredilerimizin kalan ana para ve faiz ödemelerini listele",
                    Category = PromptCategoryType.FinanceAndCashFlow,
                    CategoryName = "Finans & Nakit Akışı",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "CFO / Finans Direktörü",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Bankalara bu yıl ne kadar kredi faizi ödedik ve kalan anaparamız ne kadar?",
                        "Kredi hesaplarımızın borç dökümü ve faiz maliyetlerini listele",
                        "Banka kredilerimizin kalan ödeme takvimini ve faiz yükünü özetle"
                    },
                    ProductionUsageCount = 88,
                    Tags = new List<string> { "Banka", "Kredi", "Faiz", "Finansman Gideri" }
                },
                new()
                {
                    Id = "q-fin-03",
                    Title = "Gelecek 30 Günlük Nakit Projeksiyonu",
                    Prompt = "Önümüzdeki 30 gün içinde vadesi gelecek alacaklar ve borçlara göre net nakit açığı veya fazlamız nedir?",
                    Category = PromptCategoryType.FinanceAndCashFlow,
                    CategoryName = "Finans & Nakit Akışı",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "CFO / Genel Müdür",
                    Complexity = "İleri Düzey",
                    AlternativePhrasings = new List<string>
                    {
                        "Önümüzdeki 1 ayda kasaya girecek kesinleşmiş paralar ile yapılacak zorunlu ödemeleri karşılaştır",
                        "Gelecek 30 günün nakit akım tablosunu ve beklenen net bakiyeyi çıkar",
                        "Bu ay çek ve fatura ödemelerimizi karşılayacak nakit fazlamız olacak mı?"
                    },
                    Tags = new List<string> { "Projeksiyon", "Gelecek Nakit", "Likidite", "Bütçe" }
                },
                new()
                {
                    Id = "q-fin-04",
                    Title = "Döviz Varlıkları ve Kur Pozisyonu",
                    Prompt = "Dövizli alacaklarımız, dövizli borçlarımız ve banka döviz mevduatlarımızın net açık/fazla kur pozisyonu nedir?",
                    Category = PromptCategoryType.FinanceAndCashFlow,
                    CategoryName = "Finans & Nakit Akışı",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "Finans Direktörü / Hazine",
                    Complexity = "İleri Düzey",
                    AlternativePhrasings = new List<string>
                    {
                        "Dolar ve Euro bazında net açık kur pozisyonumuz kaç para?",
                        "Döviz borçlarımızın döviz alacaklarımıza oranı ve kur riski durumumuz nedir?",
                        "Kur artışından şirketimiz olumlu mu olumsuz mu etkilenir, net döviz dengesini göster"
                    },
                    Tags = new List<string> { "Döviz", "Kur Riski", "Hazine", "USD/EUR" }
                },
                new()
                {
                    Id = "q-fin-05",
                    Title = "Banka Hesap Bakiyeleri ve Mevduat Dağılımı",
                    Prompt = "Tüm bankalardaki vadesiz ve vadeli mevduat bakiyelerimizi döviz cinslerine göre listele",
                    Category = PromptCategoryType.FinanceAndCashFlow,
                    CategoryName = "Finans & Nakit Akışı",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Finans Müdürü",
                    Complexity = "Temel",
                    AlternativePhrasings = new List<string>
                    {
                        "Bankalarda şu an toplam kaç para nakdimiz var?",
                        "Banka bazında anlık mevduat durumumuzu ve hesap türlerini göster"
                    },
                    ProductionUsageCount = 118,
                    Tags = new List<string> { "Banka", "Mevduat", "Bakiye" }
                },
                new()
                {
                    Id = "q-fin-06",
                    Title = "Haftalık Nakit Çıkış Takvimi",
                    Prompt = "Bu hafta ödenmesi gereken çekler, senetler, personel avansları ve satıcı faturalarının toplam tutarı nedir?",
                    Category = PromptCategoryType.FinanceAndCashFlow,
                    CategoryName = "Finans & Nakit Akışı",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Finans & Muhasebe",
                    Complexity = "Temel",
                    AlternativePhrasings = new List<string>
                    {
                        "Bu haftaki kesinleşmiş nakit ödeme planımız nedir?",
                        "Önümüzdeki 7 gün içinde bankadan veya kasadan çıkacak toplam ödemeler"
                    },
                    ProductionUsageCount = 135,
                    Tags = new List<string> { "Haftalık Ödeme", "Nakit Çıkışı", "Çek Vadesi" }
                },

                // =========================================================================
                // 2. SATIŞ & CİRO
                // =========================================================================
                new()
                {
                    Id = "q-sal-01",
                    Title = "En Yüksek Tutarlı Satış Faturaları",
                    Prompt = "Son 90 gündeki en yüksek tutarlı satış faturalarını ve müşterilerini listele",
                    Category = PromptCategoryType.SalesAndRevenue,
                    CategoryName = "Satış & Ciro",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Satış Direktörü / Genel Müdür",
                    Complexity = "Temel",
                    AlternativePhrasings = new List<string>
                    {
                        "Son 3 ayda kestiğimiz en büyük 20 satış faturası kime ve kaç lira?",
                        "En yüksek cirolu satış faturalarını müşteri bazında sırala",
                        "Son çeyrekteki rekor faturalarımızı ve satılan ana ürünleri göster"
                    },
                    ProductionUsageCount = 215,
                    Tags = new List<string> { "Satış Faturası", "Büyük Müşteriler", "Ciro Liderleri" }
                },
                new()
                {
                    Id = "q-sal-02",
                    Title = "Satış Cirosu ve İade Oranı Dengesi",
                    Prompt = "Satış ciromuzu, iade tutarlarımızı ve net satış gelirini karşılaştır",
                    Category = PromptCategoryType.SalesAndRevenue,
                    CategoryName = "Satış & Ciro",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Satış & Kalite Yöneticisi",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Bu yıl yaptığımız satışların yüzde kaçı iade geldi?",
                        "Aylık brüt ciro, iade kesintileri ve net satış gelirimizin grafiği nedir?",
                        "En çok iade edilen ürünler ve iadenin toplam ciromuza maliyeti ne kadar?"
                    },
                    ProductionUsageCount = 173,
                    Tags = new List<string> { "Ciro", "İade Oranı", "Net Satış", "Kalite" }
                },
                new()
                {
                    Id = "q-sal-03",
                    Title = "Satış Hacmi Düşen Riskli Müşteriler",
                    Prompt = "Son 6 ayda bizden alımları %30'dan fazla düşen ve sipariş kesen eski müşterilerimizi listele",
                    Category = PromptCategoryType.SalesAndRevenue,
                    CategoryName = "Satış & Ciro",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "Satış Direktörü / Müşteri Başarısı",
                    Complexity = "İleri Düzey",
                    AlternativePhrasings = new List<string>
                    {
                        "Eskiye göre cirosu en çok gerileyen ve bizi terk etme riski olan müşteriler kimler?",
                        "Son 90 gündür sipariş vermeyen aktif müşterilerimizi listele",
                        "Ciro kaybı yaşadığımız ilk 15 müşteriyi ve kayıp tutarlarını göster"
                    },
                    Tags = new List<string> { "Müşteri Kaybı", "Churn Riski", "Retention", "Satış Trendi" }
                },
                new()
                {
                    Id = "q-sal-04",
                    Title = "Kâr Marjı En Yüksek Ürünler",
                    Prompt = "Satış hacminden bağımsız olarak, birim başına brüt kâr marjı en yüksek olan ilk 20 ürünümüz hangisi?",
                    Category = PromptCategoryType.SalesAndRevenue,
                    CategoryName = "Satış & Ciro",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "Genel Müdür / Pazarlama",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Bize en çok net kâr bırakan kârlılık şampiyonu ürünlerimiz nelerdir?",
                        "Ürünlerimizin maliyetine göre kâr marjı sıralamasını çıkar",
                        "Satış cirosu düşük olsa da kâr oranı çok yüksek olan gizli yıldız ürünler hangileri?"
                    },
                    Tags = new List<string> { "Kâr Marjı", "Ürün Kârlılığı", "Brüt Kâr", "Fiyatlandırma" }
                },
                new()
                {
                    Id = "q-sal-05",
                    Title = "Bu Ay En Çok Ciro Yaptığımız İlk 10 Müşteri",
                    Prompt = "Bu ay içerisinde en yüksek tutarlı alım yapan ilk 10 müşteriyi ve aldıkları ürün adetlerini getir",
                    Category = PromptCategoryType.SalesAndRevenue,
                    CategoryName = "Satış & Ciro",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Satış Müdürü",
                    Complexity = "Temel",
                    AlternativePhrasings = new List<string>
                    {
                        "Ayın en büyük müşterileri kimler ve ne kadarlık fatura kestik?",
                        "Ciro lideri ilk 10 müşteri listesi ve geçen aya göre artış durumları"
                    },
                    ProductionUsageCount = 186,
                    Tags = new List<string> { "Aylık Ciro", "Top 10 Müşteri", "Satış Liderleri" }
                },

                // =========================================================================
                // 3. MÜŞTERİLER & TAHSİLAT
                // =========================================================================
                new()
                {
                    Id = "q-rec-01",
                    Title = "Vadesi Geçmiş Açık Faturalar ve Yaşlandırma",
                    Prompt = "Vade tarihleri ve müşteri bakiyelerine göre vadesi geçmiş tahsilat risklerini analiz et",
                    Category = PromptCategoryType.CustomerAndReceivables,
                    CategoryName = "Müşteriler & Tahsilat",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Kredi & Tahsilat Müdürü / CFO",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Vadesi geçmiş ama henüz ödenmemiş faturaları gecikme gününe göre sırala",
                        "Hangi müşterimizin ne kadar gecikmiş borcu var ve kaç gündür gecikmede?",
                        "Müşteri alacaklarımızın yaşlandırma tablosunu ve riskli bakiyeleri çıkar"
                    },
                    ProductionUsageCount = 310,
                    Tags = new List<string> { "Vadesi Geçmiş", "Alacak Yaşlandırma", "Tahsilat Riski", "Açık Fatura" }
                },
                new()
                {
                    Id = "q-rec-02",
                    Title = "Karşılıksız ve Protestolu Çek/Senet Portföyü",
                    Prompt = "Karşılıksız ve protestolu çeklerimizi, asıl keşidecileri ve tutarlarını listele",
                    Category = PromptCategoryType.CustomerAndReceivables,
                    CategoryName = "Müşteriler & Tahsilat",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Hukuk & Risk Yönetimi / Finans Müdürü",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Karşılıksız çıkan müşteri çeklerimizin toplam tutarı ve borçluları kimler?",
                        "Protestolu senetlerimizi ve tahsil edilemeyen evrakları listele",
                        "Portföyümüzde sorunlu ve hukuki takibe intikal eden çeklerin dökümü"
                    },
                    ProductionUsageCount = 120,
                    Tags = new List<string> { "Karşılıksız Çek", "Protesto Senet", "İcra Takibi", "Keşideci" }
                },
                new()
                {
                    Id = "q-rec-03",
                    Title = "Ortalama Tahsilat Süresi (DSO) En Yüksek Müşteriler",
                    Prompt = "Faturalarını ortalama kaç günde ödediklerine göre, tahsilatı en yavaş ve bizi en çok finanse ettiren müşteriler kimler?",
                    Category = PromptCategoryType.CustomerAndReceivables,
                    CategoryName = "Müşteriler & Tahsilat",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "CFO / Kredi Komitesi",
                    Complexity = "İleri Düzey",
                    AlternativePhrasings = new List<string>
                    {
                        "Müşterilerimizin ortalama fatura ödeme süresi (DSO) kaç gün?",
                        "Vadesine en az uyan ve ödemeyi sürekli geciktiren carilerin sıralaması",
                        "Tahsilat hızı en yavaş ilk 20 müşteri ve ortalama gecikme süreleri"
                    },
                    Tags = new List<string> { "DSO", "Tahsilat Süresi", "Ödeme Disiplini", "Nakit Döngüsü" }
                },
                new()
                {
                    Id = "q-rec-04",
                    Title = "Kredi Limitini Aşan Riskli Müşteriler",
                    Prompt = "Tanımlı risk limitini veya açık hesap sınırını aşan müşterileri ve güncel aşım tutarlarını listele",
                    Category = PromptCategoryType.CustomerAndReceivables,
                    CategoryName = "Müşteriler & Tahsilat",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "Kredi Risk Müdürü",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Risk limiti dolan müşterilere yeni satış yapılmış mı?",
                        "Açık hesap limitini aşan müşterilerin listesi ve teminat durumları"
                    },
                    Tags = new List<string> { "Risk Limiti", "Kredi Limiti", "Teminat" }
                },

                // =========================================================================
                // 4. STOK & ENVANTER
                // =========================================================================
                new()
                {
                    Id = "q-stk-01",
                    Title = "Kritik Minimum Stok Seviyesinin Altındaki Ürünler",
                    Prompt = "Kritik ve minimum stok seviyesinin altına düşen ürünleri, mevcut miktarını ve ambarlarını listele",
                    Category = PromptCategoryType.StockAndInventory,
                    CategoryName = "Stok & Envanter",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Satınalma & Depo Müdürü",
                    Complexity = "Temel",
                    AlternativePhrasings = new List<string>
                    {
                        "Stoklarımızda tükenmek üzere olan ve acil sipariş verilmesi gereken ürünler hangileri?",
                        "Depolarımızda minimum seviyenin altına inen kritik malzemeleri getir",
                        "Hangi ürünlerin stoğu kritik eşiğe geldi ve kaç adet kaldı?"
                    },
                    ProductionUsageCount = 195,
                    Tags = new List<string> { "Minimum Stok", "Kritik Seviye", "Ambar", "Tedarik" }
                },
                new()
                {
                    Id = "q-stk-02",
                    Title = "En Yüksek Envanter Değerine Sahip İlk 20 Ürün",
                    Prompt = "Stoklarımızda toplam envanter maliyeti ve tutarı en yüksek olan ilk 20 ürünü göster",
                    Category = PromptCategoryType.StockAndInventory,
                    CategoryName = "Stok & Envanter",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Tedarik Zinciri Direktörü / CFO",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Depoda en çok para bağladığımız en pahalı stok kalemleri nelerdir?",
                        "Toplam envanter değerimizin en büyük kısmını oluşturan ilk 20 malzeme",
                        "Stoktaki toplam maliyet tutarına göre ürün sıralaması çıkar"
                    },
                    ProductionUsageCount = 110,
                    Tags = new List<string> { "Envanter Değeri", "ABC Analizi", "Stok Maliyeti", "Depo" }
                },
                new()
                {
                    Id = "q-stk-03",
                    Title = "Hareketsiz Yatan Ölü / Atıl Stoklar",
                    Prompt = "Son 90 gündür ne satılan ne de üretime giren, depoda hareketsiz yatan ölü stokları ve bağlanan toplam parayı listele",
                    Category = PromptCategoryType.StockAndInventory,
                    CategoryName = "Stok & Envanter",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "Satış & Depo Direktörü",
                    Complexity = "İleri Düzey",
                    AlternativePhrasings = new List<string>
                    {
                        "Depomuzda atıl duran, hiç satılmayan ölü stokların listesi ve maliyet değeri nedir?",
                        "Son 3 aydır hareket görmeyen malzemeler toplamda kaç liralık sermaye bağlıyor?",
                        "Yavaş hareket eden (Slow-Moving) ve hareketsiz envanter dökümünü çıkar"
                    },
                    Tags = new List<string> { "Ölü Stok", "Atıl Envanter", "Tasfiye", "Nakit Dönüşümü" }
                },

                // =========================================================================
                // 5. SİPARİŞ & SEVKİYAT
                // =========================================================================
                new()
                {
                    Id = "q-ord-01",
                    Title = "Bekleyen Açık Müşteri Siparişleri",
                    Prompt = "Teslimatı bekleyen açık müşteri siparişlerini, kalan miktarları ve teslim tarihlerini listele",
                    Category = PromptCategoryType.OrdersAndLogistics,
                    CategoryName = "Sipariş & Sevkiyat",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Operasyon & Sevkiyat Müdürü",
                    Complexity = "Temel",
                    AlternativePhrasings = new List<string>
                    {
                        "Şu an depoda sevk edilmeyi bekleyen bekleyen açık siparişlerimizin toplamı ne kadar?",
                        "Müşterilerin bekleyen açık sipariş dökümünü teslimat tarihlerine göre sırala",
                        "Henüz faturası kesilip teslim edilmemiş siparişler ve teslim taahhütleri"
                    },
                    ProductionUsageCount = 165,
                    Tags = new List<string> { "Açık Sipariş", "Sevkiyat", "Teslimat", "Bekleyen Sipariş" }
                },
                new()
                {
                    Id = "q-ord-02",
                    Title = "Teslim Tarihi Geçmiş Geciken Siparişler",
                    Prompt = "Müşteriye taahhüt edilen teslim tarihi geçmiş olan fakat henüz sevk edilememiş geciken siparişleri listele",
                    Category = PromptCategoryType.OrdersAndLogistics,
                    CategoryName = "Sipariş & Sevkiyat",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "Müşteri Hizmetleri & Lojistik Müdürü",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Termin süresi dolduğu halde müşteriye gönderilemeyen gecikmiş siparişlerimiz hangileri?",
                        "Teslimatı geciken siparişlerin tutarı ve gecikme süreleri nedir?",
                        "Gecikmede olan müşteri teslimatlarını ve aciliyet durumunu listele"
                    },
                    Tags = new List<string> { "Geciken Sipariş", "Termin Süresi", "Sevkiyat Gecikmesi", "OTIF" }
                },

                // =========================================================================
                // 6. YÖNETİM & STRATEJİ
                // =========================================================================
                new()
                {
                    Id = "q-exe-01",
                    Title = "Ödenmiş ve Ödenmemiş Fatura Dengesi",
                    Prompt = "Aylara göre ödenmiş ve ödenmemiş fatura toplamlarını karşılaştır",
                    Category = PromptCategoryType.ExecutiveSummaries,
                    CategoryName = "Yönetim & Strateji",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Genel Müdür / Yönetim Kurulu",
                    Complexity = "Orta",
                    AlternativePhrasings = new List<string>
                    {
                        "Aylık gelir ve ödenmemiş fatura trendlerimizi özetle",
                        "Kestiğimiz faturaların kaç lirası fiilen tahsil edildi, kaç lirası açıkta bekliyor?",
                        "Aylık tahsilat başarı oranımızın fatura kesim hızına göre grafiği nedir?"
                    },
                    ProductionUsageCount = 280,
                    Tags = new List<string> { "Fatura Karşılaştırma", "Tahsilat Oranı", "Aylık Gelir", "C-Level" }
                },
                new()
                {
                    Id = "q-exe-02",
                    Title = "Çeyreklik Ciro ve Büyüme Trendi",
                    Prompt = "Mali çeyrekler (Q1, Q2, Q3, Q4) bazında satış ciromuzu ve çeyreklik trendi özetle",
                    Category = PromptCategoryType.ExecutiveSummaries,
                    CategoryName = "Yönetim & Strateji",
                    OriginType = QuestionOriginType.RealTenantAsked,
                    OriginLabel = "Popüler",
                    TargetRole = "Strateji & Bütçe Planlama / CEO",
                    Complexity = "İleri Düzey",
                    AlternativePhrasings = new List<string>
                    {
                        "Bu yılın çeyrek dönemleri (Q1, Q2, Q3, Q4) arasındaki ciro büyüme performansımız nasıl?",
                        "Son 2 yılın çeyreklik satış kıyaslaması ve trend analizi nedir?",
                        "Yıllık bütçe hedeflerimizin çeyrek bazında gerçekleşme oranları"
                    },
                    ProductionUsageCount = 155,
                    Tags = new List<string> { "Çeyreklik Büyüme", "Q1-Q4", "Mali Takvim", "Yönetim Kurulu" }
                },
                new()
                {
                    Id = "q-exe-03",
                    Title = "Pareto Analizi: Kârımızın %80'ini Üreten VIP Müşteriler",
                    Prompt = "Şirketimizin toplam brüt kârının %80'ini üreten ilk %20'lik ana müşteri kitlemiz kimlerdir?",
                    Category = PromptCategoryType.ExecutiveSummaries,
                    CategoryName = "Yönetim & Strateji",
                    OriginType = QuestionOriginType.StrategicRecommendation,
                    OriginLabel = "Öneri",
                    TargetRole = "Yönetim Kurulu Başkanı / CEO",
                    Complexity = "İleri Düzey",
                    AlternativePhrasings = new List<string>
                    {
                        "80/20 kuralına göre kârımızın ezici çoğunluğunu sırtlayan VIP müşterilerimiz kimler?",
                        "Cirodan bağımsız olarak bize en çok para kazandıran çekirdek müşteri portföyü",
                        "Şirketimizin kârlılık omurgasını oluşturan ilk 15 müşteri analizi"
                    },
                    Tags = new List<string> { "Pareto 80/20", "VIP Müşteri", "Kârlılık Omurgası", "Stratejik Karar" }
                }
            };
        }
    }
}
