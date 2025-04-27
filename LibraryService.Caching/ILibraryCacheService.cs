namespace LibraryService.Caching
{
    public interface ILibraryCacheService
    {
        Task<T> GetAsync<T>(object key);
        Task SetAsync<T>(object key, T value);
        Task RemoveAsync(object key);
    }
}
