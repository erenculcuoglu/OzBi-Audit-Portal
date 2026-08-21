using System.Collections.Generic;
using System.Threading.Tasks;
using OzBiPortalCRM.Models;

namespace OzBiPortalCRM.Services
{
    public interface IPromptTemplateService
    {
        Task<List<PromptTemplate>> GetAllQuestionsAsync(
            PromptCategoryType category = PromptCategoryType.All,
            QuestionOriginType originType = QuestionOriginType.All,
            string? targetRole = null,
            string? searchTerm = null,
            string? complexity = null,
            int portalUserId = 0);

        Task<List<TemplateCategoryInfo>> GetCategoriesSummaryAsync();
        List<TemplateCategoryInfo> GetCategoriesSummary(List<PromptTemplate> preloaded);

        Task<PromptTemplate?> GetQuestionByIdAsync(string id, int portalUserId = 0);

        Task<bool> AddCustomQuestionAsync(CustomPromptTemplateItem item);

        Task<bool> DeleteCustomQuestionAsync(string id, int portalUserId);

        Task<Dictionary<string, int>> GetQuestionStatsAsync();

        /// <summary>
        /// Logo ve Mikro ERP işlevsel modellerine dayalı olarak sıfırdan dinamik yeni sorular üretir.
        /// </summary>
        Task<List<PromptTemplate>> GenerateFreshDynamicQuestionsAsync(
            int count = 8, 
            PromptCategoryType category = PromptCategoryType.All, 
            int portalUserId = 0);
    }
}
