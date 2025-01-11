using LocationApp.Account.Hubs;
using LocationApp.Model.Core;
using LocationApp.Model.Extensions;
using LocationApp.Model.JsonModels;
using LocationApp.Repository;
using LocationApp.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace LocationApp.Account.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IHubContext<SearchHub> _hubContext;

        public AuthenticationController(IRepositoryManager repositoryManager, IHubContext<SearchHub> hubContext)
        {
            _repositoryManager = repositoryManager;
            _hubContext = hubContext;
        }

        [HttpPost("Login")]
        [SwaggerOperation(summary: "User login")]
        [SwaggerResponse(200, "User authentication data")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromServices] RequestQueueService requestQueue, 
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, 
            [FromBody] LoginInput input)
        {
            // Check if the Idempotency-Key is present
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                return BadRequest("Idempotency-Key is required.");

            // Create key for queue
            var operationKey = "LoginOperation" + input.Username;

            // Enqueue the operation
            return await requestQueue.EnqueueAsync<IActionResult>(operationKey, async () =>
            {
                // Get user from the repository
                var user = await _repositoryManager.User.GetUser(input.Username, input.Password);

                if (user is null)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Send notification to console
                await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"User id {user.Id} logged in");

                return Ok(new { ApiKey = user.ApiKey });
            });
        }

        [HttpPost("Register")]
        [SwaggerOperation(summary: "User registration")]
        [SwaggerResponse(200, "User authentication data")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(
            [FromServices] RequestQueueService requestQueue, 
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, 
            [FromBody]RegistrationInput input)
        {
            // Check if the Idempotency-Key is present
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                return BadRequest("Idempotency-Key is required.");

            // Create key for queue
            var operationKey = "RegisterOperation" + input.Username;

            // Enqueue the operation
            return await requestQueue.EnqueueAsync<IActionResult>(operationKey, async () =>
            {
                // Check if the username is already taken
                if (await _repositoryManager.User.CheckUserExists(input.Username))
                    return BadRequest("Username already taken");

                // Hash the password
                var passwordHash = input.Password.ToSHA512Hash();
                // Generate API key
                var apiKey = Guid.NewGuid().ToString();

                // Create new user
                var newUser = new User
                {
                    Username = input.Username,
                    Password = passwordHash,
                    ApiKey = apiKey
                };

                // Save user to the repository
                await _repositoryManager.User.CreateUser(newUser);
                await _hubContext.Clients.All.SendAsync("ReceiveSearchNotification", $"User registared with username {newUser.Username}");

                return Ok(new { ApiKey = apiKey });
            });
        }
    }
}
