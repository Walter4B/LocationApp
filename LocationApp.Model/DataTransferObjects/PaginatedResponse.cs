namespace LocationApp.Model.DataTransferObjects
{
    public class PaginatedResponse<T>
    {
        public int ItemCount { get; set; }
        public List<T>? Items { get; set; }
    }
}
