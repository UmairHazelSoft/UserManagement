using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UserManagementSystem.DTOs;

namespace UserManagementSystem.Helpers
{
    public static class QueryablePagingExtensions
    {
        public static async Task<PagedResultDto<T>> ApplyPagingAsync<T>(
            this IQueryable<T> query,
            GenericPaginationParams pagination
        )
        {
            //  Decode filters (BL concern)
            if (!string.IsNullOrWhiteSpace(pagination.Base64Filters))
            {
                var whereClause = Encoding.UTF8.GetString(
                    Convert.FromBase64String(pagination.Base64Filters)
                );

                ValidateDynamicWhereClause<T>(whereClause);  // validating the where clause
                query = query.Where(whereClause);
            }

            // Sorting
            query = ApplySorting(query, pagination.SortColumn, pagination.SortDirection);

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

        private static IQueryable<T> ApplySorting<T>(
            IQueryable<T> query,
            string? sortColumn,
            string? sortDirection
        )
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
                return query;

            var prop = typeof(T).GetProperty(
                sortColumn,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (prop == null)
                throw new ArgumentException($"Invalid sort column: {sortColumn}");

            var dir = sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true
                ? "descending"
                : "ascending";

            return query.OrderBy($"{prop.Name} {dir}");
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

    }

}
