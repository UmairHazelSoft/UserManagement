using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using UserManagementSystem.DbContext;
using UserManagementSystem.DTOs;
using UserManagementSystem.Enums;

namespace UserManagementSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext dbContext)
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


        public async Task<PagedResult<TDto>> GetPagedAsync<TDto>(
            GenericPaginationParams pagination,
            Func<T, TDto> mapToDto
        ) where TDto : class
        {
            try
            {
                IQueryable<T> query = _dbSet.AsQueryable();

                List<GenericFilter> filters = new();
                if (!string.IsNullOrEmpty(pagination.Base64Filters))
                {
                    var json = Encoding.UTF8.GetString(Convert.FromBase64String(pagination.Base64Filters));

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    options.Converters.Add(new JsonStringEnumConverter()); // this is key

                    filters = JsonSerializer.Deserialize<List<GenericFilter>>(json, options)
                              ?? new List<GenericFilter>();
                }

               
                foreach (var filter in filters)
                {
                    if (string.IsNullOrEmpty(filter.Column) || string.IsNullOrEmpty(filter.Value))
                        continue;

                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.PropertyOrField(parameter, filter.Column);
                    var constant = Expression.Constant(filter.Value);
                    Expression? predicate = null;


                    if (property.Type == typeof(string))
                    {
                        switch (filter.Operator)
                        {
                            case FilterOperatorEnum.Contains:
                                predicate = Expression.Call(property, typeof(string)
                                    .GetMethod("Contains", new[] { typeof(string) })!, constant);
                                break;
                            case FilterOperatorEnum.Equals:
                                predicate = Expression.Equal(property, constant);
                                break;
                            case FilterOperatorEnum.StartsWith:
                                predicate = Expression.Call(property, typeof(string)
                                    .GetMethod("StartsWith", new[] { typeof(string) })!, constant);
                                break;
                            case FilterOperatorEnum.EndsWith:
                                predicate = Expression.Call(property, typeof(string)
                                    .GetMethod("EndsWith", new[] { typeof(string) })!, constant);
                                break;
                            default:
                                break;
                        }

                    }
                    else if (property.Type == typeof(bool) && bool.TryParse(filter.Value, out bool boolValue))
                    {
                        predicate = Expression.Equal(property, Expression.Constant(boolValue));
                    }
                    else if (property.Type == typeof(int) && int.TryParse(filter.Value, out int intValue))
                    {
                        predicate = Expression.Equal(property, Expression.Constant(intValue));
                    }

                    if (predicate != null)
                    {
                        var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
                        query = query.Where(lambda);
                    }
                }

                if (!string.IsNullOrEmpty(pagination.SortColumn))
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var property = Expression.PropertyOrField(parameter, pagination.SortColumn);
                    var lambda = Expression.Lambda(property, parameter);

                    string methodName = pagination.SortDirection?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";

                    var resultExp = Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new Type[] { typeof(T), property.Type },
                        query.Expression,
                        Expression.Quote(lambda)
                    );

                    query = query.Provider.CreateQuery<T>(resultExp);
                }

                
                var totalCount = await query.CountAsync();

               
                var items = await query
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();

                
                var dtoItems = items.Select(mapToDto).ToList();

                return new PagedResult<TDto>
                {
                    Items = dtoItems,
                    TotalCount = totalCount,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }






    }
}
