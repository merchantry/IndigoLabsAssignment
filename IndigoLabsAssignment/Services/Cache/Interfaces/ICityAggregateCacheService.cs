using IndigoLabsAssignment.Models;

namespace IndigoLabsAssignment.Services.Cache.Interfaces
{
    public interface ICityAggregateCacheService
    {
        bool ShouldRefresh(FileMetaData fileMetaData);
        Dictionary<string, CityAggregate> Get();
        void Set(FileMetaData fileMetaData, Dictionary<string, CityAggregate> value);
    }
}
