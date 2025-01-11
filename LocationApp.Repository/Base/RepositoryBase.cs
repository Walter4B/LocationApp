using LocationApp.Model.Core;

namespace LocationApp.Repository.Base
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext) => RepositoryContext = repositoryContext;

        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
        public void CreateRange(IEnumerable<T> entities) => RepositoryContext.Set<T>().AddRange(entities);
        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
        public void UpdateRange(IEnumerable<T> entities) => RepositoryContext.Set<T>().UpdateRange(entities);
        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);
    }
}
