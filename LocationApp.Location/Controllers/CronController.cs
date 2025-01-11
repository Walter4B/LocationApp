using LocationApp.Model.Core;
using LocationApp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace LocationApp.Location.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CronController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public CronController(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpPost]
        [SwaggerOperation(summary: "Insert locations in database")]
        [SwaggerResponse(200, "Locations inserted")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> InsertLocations()
        {
            List<Model.Core.Location> locations = new List<Model.Core.Location>();
            List<Category> categories = new List<Category>();
            Dictionary<string, List<string>> locationTypesDict = new Dictionary<string, List<string>>();

            HttpClient client = new HttpClient();
            string apiKey = "YOUR_API_KEY";

            string coordinates = "45.8150,15.9819";
            int radius = 5000;
            string nextPageToken = null;

            do
            {
                // Construct the URL with next_page_token if available
                string requestUrl = string.IsNullOrEmpty(nextPageToken)
                    ? $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={coordinates}&radius={radius}&key={apiKey}"
                    : $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?pagetoken={nextPageToken}&key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                string responseData = await response.Content.ReadAsStringAsync();

                var googlePlaces = JsonConvert.DeserializeObject<GooglePlaceResponse>(responseData);
                foreach (var result in googlePlaces.Results.Where(r => r.Place_Id != null && r.Rating != 0 && !locations.Any(l => r.Place_Id == l.ExternalPlaceId)))
                {
                    // Add location to location list
                    locations.Add(new Model.Core.Location()
                    {
                        ExternalPlaceId = result.Place_Id,
                        Name = result.Name,
                        Address = result.Vicinity,
                        GeoLocation = new NetTopologySuite.Geometries.Point(result.Geometry.Location.Lng, result.Geometry.Location.Lat) { SRID = 4326 },
                        Rating = result.Rating,
                        RatingsTotal = result.User_Ratings_Total,
                    });

                    // Add location types to category list
                    foreach (var type in result.Types)
                    {
                        if (!categories.Any(c => c.Name == type))
                        {
                            categories.Add(new Category() { Name = type });
                        }
                    }

                    locationTypesDict.Add(result.Place_Id, result.Types);
                }

                // Update the nextPageToken (it might be null if no more pages are available)
                nextPageToken = googlePlaces.Next_Page_Token;

                // Pause to comply with Google's API requirement to wait before using the token
                if (!string.IsNullOrEmpty(nextPageToken))
                {
                    await Task.Delay(2000);
                }

            } while (!string.IsNullOrEmpty(nextPageToken));

            List<Category> createdCategories = await _repositoryManager.Category.CreateCategories(categories);

            foreach (var location in locations)
            {
                location.Categories = categories.Where(c => locationTypesDict[location.ExternalPlaceId].Contains(c.Name)).ToList();
            }

            await _repositoryManager.Location.CreateLocations(locations);

            return Ok();
        }

        #region GooglePlacesObjects
        private class GooglePlaceResponse
        {
            public List<Result> Results { get; set; }
            public string Next_Page_Token { get; set; }
        }

        private class Result
        {
            public string Place_Id { get; set; }
            public string Name { get; set; }
            public string Vicinity { get; set; }
            public decimal Rating { get; set; }
            public int User_Ratings_Total { get; set; }
            public Geometry Geometry { get; set; }
            public List<string> Types { get; set; }
        }

        private class Geometry
        {
            public Location Location { get; set; }
        }

        private class Location
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
        #endregion
    }
}
