using UserManagementSystem.Enums;

namespace UserManagementSystem.DTOs
{
    public class GenericFilter
    {
        public string Column { get; set; } = string.Empty; 
        public FilterOperatorEnum Operator { get; set; }  
        public string Value { get; set; } = string.Empty;  
    }
}
