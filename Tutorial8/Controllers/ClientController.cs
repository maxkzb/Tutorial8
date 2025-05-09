using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        /// <summary>
        /// Retrieves all trips assigned to a client by their ID.
        /// </summary>
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrip(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest("Client ID must be a positive integer.");

            var trips = await _clientsService.getClientTripsByIdAsync(id, cancellationToken);

            if (trips == null || !trips.Any())
                return NotFound($"No trips found for client ID {id}.");

            return Ok(trips);
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] Client newClient, CancellationToken cancellationToken)
        {
            // Basic validation of required fields
            if (newClient == null ||
                string.IsNullOrWhiteSpace(newClient.FirstName) ||
                string.IsNullOrWhiteSpace(newClient.LastName) ||
                string.IsNullOrWhiteSpace(newClient.Email) ||
                string.IsNullOrWhiteSpace(newClient.Pesel) ||
                string.IsNullOrWhiteSpace(newClient.Telephone))
            {
                return BadRequest("All fields (FirstName, LastName, Email, Pesel, Telephone) are required.");
            }

            var newClientId = await _clientsService.CreateClientAsync(newClient, cancellationToken);

            if (newClientId is null)
            {
                return NotFound("Client could not be created.");
            }

            return Created(string.Empty, new { IdClient = newClientId });
        }

        /// <summary>
        /// Registers a client to a specific trip.
        /// </summary>
        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> RegisterClientToTrip(int id, int tripId, CancellationToken cancellationToken)
        {
            if (id <= 0 || tripId <= 0)
                return BadRequest("Client ID and Trip ID must be positive integers.");

            var result = await _clientsService.RegisterClientToTripAsync(id, tripId, cancellationToken);

            return result switch
            {
                "ClientNotFound" => NotFound("Client not found."),
                "TripNotFound" => NotFound("Trip not found."),
                "TripFull" => BadRequest("Trip has reached the maximum number of participants."),
                "AlreadyRegistered" => Conflict("Client is already registered for this trip."),
                "Success" => Ok("Client registered to trip successfully."),
                _ => StatusCode(500, "An error occurred while registering the client.")
            };
        }

        /// <summary>
        /// Unregisters a client from a specific trip.
        /// </summary>
        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> UnregisterClientFromTrip(int id, int tripId, CancellationToken cancellationToken)
        {
            if (id <= 0 || tripId <= 0)
                return BadRequest("Client ID and Trip ID must be positive integers.");

            var result = await _clientsService.UnregisterClientFromTripAsync(id, tripId, cancellationToken);

            return result switch
            {
                "NotRegistered" => NotFound("Registration not found."),
                "Success" => Ok("Client successfully unregistered from trip."),
                _ => StatusCode(500, "An error occurred while unregistering the client.")
            };
        }
    }
}
