using AutoMapper;
using LocationApp.Account.Hubs;
using LocationApp.Model.DataTransferObjects;
using LocationApp.Model.Extensions;
using LocationApp.Model.Helper;
using LocationApp.Model.JsonModels;
using LocationApp.Repository;
using LocationApp.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NetTopologySuite.Geometries;
using Swashbuckle.AspNetCore.Annotations;

namespace LocationApp.Location.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly RequestQueueService _requestQueue;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IHubContext<SearchHub> _hubContext;
        private readonly IMapper _mapper;

        public LocationController(RequestQueueService requestQueue, IRepositoryManager repositoryManager, IHubContext<SearchHub> hubContext, IMapper mapper)
        {
            _requestQueue = requestQueue;
            _repositoryManager = repositoryManager;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        [HttpGet("Categories")]
        [SwaggerOperation(summary: "Get locations")]
        [SwaggerResponse(200, "Got locations")]
        public async Task<IActionResult> GetCategories(
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
            [FromHeader(Name = "Authorization")] string apiKey,
            [FromQuery] DataTableOptions input)
        {
            return await _requestQueue.EnqueueAsync<IActionResult>(apiKey, async () =>
            {
                var categories = await _repositoryManager.Category.GetCategories(input);

                var response = new PaginatedResponse<CategoryDto>
                {
                    ItemCount = categories.TotalRecords,
                    Items = _mapper.Map<List<CategoryDto>>(categories.Responses),
                };

                await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", "Got categories");
                return Ok(response);
            });
        }

        [HttpGet]
        [SwaggerOperation(summary: "Get locations")]
        [SwaggerResponse(200, "Got locations")]
        public async Task<IActionResult> GetLocations(
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
            [FromHeader(Name = "Authorization")] string apiKey,
            [FromQuery] LocationInput input)
        {
            return await _requestQueue.EnqueueAsync<IActionResult>(apiKey, async () =>
            {
                Geometry currenctLocation = new Point(input.Latitude, input.Longitude) { SRID = 4326 };

                var locations = await _repositoryManager.Location.GetLocations(input, apiKey.GetUserApiKey());

                var response = new PaginatedResponse<LocationDto>
                {
                    ItemCount = locations.TotalRecords,
                    Items = _mapper.Map<List<LocationDto>>(locations.Responses),
                };

                await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", "Got locations");
                return Ok(response);
            });
        }

        [HttpPut("Favorite")]
        [SwaggerOperation(summary: "Change location favoriete status", description: "If location is not in user favorites add it else remove it")]
        [SwaggerResponse(200, "Changed location favorite status")]
        public async Task<IActionResult> FavoriteStatus(
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
            [FromHeader(Name = "Authorization")] string apiKey,
            [FromQuery] int locationId)
        {
            return await _requestQueue.EnqueueAsync<IActionResult>(apiKey, async () =>
            {
                // Get the user from the repository
                var user = await _repositoryManager.User.GetUserWithLocations(apiKey.GetUserApiKey());

                // Check if the user exists
                if (user == null)
                    return Unauthorized("Invalid user");

                if (user.Locations.Any(l => l.Id.Equals(locationId)))
                {
                    // Remove the location from the user's favorites
                    user.Locations.Remove(user.Locations.First(l => l.Id.Equals(locationId)));
                    await _repositoryManager.SaveAsync();

                    // Send notification to console
                    await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"User Id {user.Id} removed location Id {locationId} from favorites");
                }
                else
                {
                    var locationToAdd = await _repositoryManager.Location.GetLocation(locationId);

                    if (locationToAdd is null)
                        return NotFound("Location not found");

                    //  Add the location to the user's favorites
                    user.Locations.Add(locationToAdd);
                    await _repositoryManager.SaveAsync();

                    // Send notification to console
                    await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"User Id {user.Id} added location Id {locationId} to favorites");
                }

                return Ok();
            });
        }

        
    }
}
