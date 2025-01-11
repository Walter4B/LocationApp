namespace LocationApp.Repository.Base
{
    internal interface IRepositoryBase<T>
    {
        void Create(T entity);
        void CreateRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
    }
}
