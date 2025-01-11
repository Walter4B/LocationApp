namespace LocationApp.Model.Helper
{
    public class DataTableOptions
    {
        public string? Search { get; set; } = "";
        public int PerPage { get; set; } = 50;
        public int Page { get; set; } = 1;
        public string SortBy { get; set; } = "Id";
        public string SortByDirection { get; set; } = "desc";
    }
}
