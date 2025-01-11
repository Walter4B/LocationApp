using LocationApp.Model.Core;
using LocationApp.Model.Extensions;
using LocationApp.Model.Helper;
using LocationApp.Repository.Base;
using LocationApp.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LocationApp.Repository.Implementations
{
    internal class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<TableResponse<Category>> GetCategories(DataTableOptions options)
        {
            var searchTerm = options.Search?.ToLower() ?? "";

            var categories = _repositoryContext.Categories.Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{searchTerm}%"));

            return new TableResponse<Category>
            {
                Responses = await categories.OrderByPage(options.SortBy, options.SortByDirection, options.Page, options.PerPage).ToListAsync(),
                TotalRecords = await categories.CountAsync(),
                Options = options
            };
        }

        public async Task<List<Category>> CreateCategories(List<Category> categories)
        {
            CreateRange(categories);
            await _repositoryContext.SaveChangesAsync();

            return categories;
        }
    }
}
