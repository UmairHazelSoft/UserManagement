using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UserManagementSystem.DbContext;
using UserManagementSystem.DTOs;

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

        public void Remove(T entity) => _dbSet.Remove(entity);

        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();





        public async Task<PagedResultDto<T>> GetPagedAsync(
            GenericPaginationParams pagination
        )
        {
            try
            {
                IQueryable<T> query = _dbSet.AsQueryable();

                // 🔹 Filtering
                if (!string.IsNullOrWhiteSpace(pagination.Base64Filters))
                {
                    var whereClause = Encoding.UTF8.GetString(
                        Convert.FromBase64String(pagination.Base64Filters)
                    );

                    ValidateDynamicWhereClause<T>(whereClause);

                    query = query.Where(whereClause);
                }

                
                query = ApplySorting(
                    query,
                    pagination.SortColumn,
                    pagination.SortDirection
                );

                
                var totalCount = await query.CountAsync();

                
                var items = await query
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();

                return new PagedResultDto<T>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "Dynamic LINQ query execution failed",
                    ex
                );
            }
        }



        public static void ValidateDynamicWhereClause<T>(string whereClause)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
                return;

            // All public properties of entity T
            var entityProperties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Extract all identifiers (words)
            var identifiers = Regex.Matches(
                whereClause,
                @"\b[A-Za-z_][A-Za-z0-9_]*\b"
            );

            foreach (Match match in identifiers)
            {
                var identifier = match.Value;

                // If identifier matches an entity property → OK
                if (entityProperties.Contains(identifier))
                    continue;

                var prop = typeof(T).GetProperty(
                    identifier,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                );

                if (prop == null && entityProperties.Contains(identifier) == false)
                {
                    // Let Dynamic LINQ throw detailed error
                    continue;
                }
            }
        }


        public static IQueryable<T> ApplySorting<T>(
        IQueryable<T> query,
        string? sortColumn,
        string? sortDirection
    )
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
                return query;

            // Validate sort column exists on entity
            var property = typeof(T).GetProperty(
                sortColumn,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (property == null)
                throw new ArgumentException($"Invalid sort column: {sortColumn}");

            var direction = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                ? "descending"
                : "ascending";

            return query.OrderBy($"{property.Name} {direction}");
        }

    }
}
