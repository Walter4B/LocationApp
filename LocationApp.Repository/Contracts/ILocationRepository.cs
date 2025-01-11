using LocationApp.Model.Core;
using LocationApp.Model.Helper;
using LocationApp.Model.JsonModels;

namespace LocationApp.Repository.Contracts
{
    public interface ILocationRepository
    {
        /// <summary>
        /// Get location by Id
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        Task<Location?> GetLocation(int locationId);

        /// <summary>
        /// Get locations in radius of the user
        /// User is included in thair favorite locations
        /// </summary>
        /// <param name="options"></param>
        /// <param name="userApiKey"></param>
        /// <returns></returns>
        Task<TableResponse<Location>> GetLocations(LocationInput options, string userApiKey);

        /// <summary>
        /// Create a list of locations
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        Task CreateLocations(List<Location> locations);
    }
}
