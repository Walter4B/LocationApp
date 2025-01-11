using LocationApp.Model.Core;
using LocationApp.Model.Extensions;
using LocationApp.Model.Helper;
using LocationApp.Model.JsonModels;
using LocationApp.Repository.Base;
using LocationApp.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace LocationApp.Repository.Implementations
{
    public class LocationRepository : RepositoryBase<Model.Core.Location>, ILocationRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public LocationRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<Model.Core.Location?> GetLocation(int locationId)
            => await _repositoryContext.Locations.Where(l => l.Id.Equals(locationId)).FirstOrDefaultAsync();

        public async Task<TableResponse<Model.Core.Location>> GetLocations(LocationInput options, string userApiKey)
        {
            var searchTerm = options.Search?.ToLower() ?? "";

            // Create a geo point for the user location
            var userLocation = new Point(options.Longitude, options.Latitude) { SRID = 4326 };

            var x = await _repositoryContext.Locations.FirstOrDefaultAsync();

            var y = x.GeoLocation.Distance(userLocation);

            var locations = _repositoryContext.Locations
                .Include(l => l.Users.Where(u => u.ApiKey.Equals(userApiKey)))
                .Include(l => l.Categories)
                .Where(l => (options.Categories.Any() ? l.Categories.Any(c => options.Categories.Contains(c.Id)) : true) &&
                    EF.Functions.Like(l.Name.ToLower(), $"%{searchTerm}%") &&
                    l.GeoLocation != null &&
                    l.GeoLocation.Distance(userLocation) <= options.Radius);

            return new TableResponse<Model.Core.Location>
            {
                Responses = await locations.OrderByPage(options.SortBy, options.SortByDirection, options.Page, options.PerPage).ToListAsync(),
                TotalRecords = await locations.CountAsync(),
                Options = options
            };
        }

        public async Task CreateLocations(List<Model.Core.Location> locations)
        {
            CreateRange(locations);
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
