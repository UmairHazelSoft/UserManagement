namespace UserManagementSystem.DTOs
{
    public class GenericPaginationParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; } = "asc";
        public string? Base64Filters { get; set; } // Frontend sends filters in base64
    }
}
