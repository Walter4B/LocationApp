using LocationApp.Model.Core;
using LocationApp.Model.Helper;

namespace LocationApp.Repository.Contracts
{
    public interface ICategoryRepository
    {
        /// <summary>
        /// Get paginated categories
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<TableResponse<Category>> GetCategories(DataTableOptions options);

        /// <summary>
        /// Create category list and return it 
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        Task<List<Category>> CreateCategories(List<Category> categories);
    }
}
