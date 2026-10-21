namespace UserManager.DTOs
{
    public class UserQueryParameters
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? SortBy { get; set; } = "date_desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
