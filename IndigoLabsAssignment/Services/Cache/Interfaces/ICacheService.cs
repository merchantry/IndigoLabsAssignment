namespace IndigoLabsAssignment.Services.Cache.Interfaces
{
    public interface ICacheService
    {
        bool Exists(string key);
        void Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);
        T? Get<T>(string key);
        void Remove(string key);
    }
}
