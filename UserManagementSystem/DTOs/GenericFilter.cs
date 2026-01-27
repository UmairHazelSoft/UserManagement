using UserManagementSystem.Enums;

namespace UserManagementSystem.DTOs
{
    public class GenericFilter
    {
        public string Column { get; set; } = string.Empty; // Column name
        public FilterOperatorEnum Operator { get; set; }  // use enum now
        public string Value { get; set; } = string.Empty;  // Value to compare
    }
}
