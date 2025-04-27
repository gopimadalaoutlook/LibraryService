namespace LibraryService.WebAPI.DTO
{
    public class ApiGetListResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Count { get; set; }
    }
}
