namespace LocationApp.Model.Helper
{
    public class TableResponse<T>
    {
        public List<T> Responses { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public DataTableOptions Options { get; set; }
    }
}
