using Microsoft.EntityFrameworkCore;
using UserManagementSystem.DbContext;
using UserManagementSystem.Helpers;

namespace UserManagementSystem.Repositories.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity)
        {
            
            var property = typeof(T).GetProperty(AppSettings.Deleted);
            if (property == null)
                throw new InvalidOperationException("Entity does not have a Deleted property.");

            property.SetValue(entity, true);
            _dbSet.Update(entity); // mark entity as updated so EF will save it
        }

        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
