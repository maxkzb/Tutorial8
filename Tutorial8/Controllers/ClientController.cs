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

        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetTrip(int id, CancellationToken cancellationToken)
        {
            var trips = await _clientsService.getClientTripsByIdAsync(id, cancellationToken);
            
            return Ok(trips);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] Client newClient, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(newClient.FirstName) ||
                string.IsNullOrEmpty(newClient.LastName) ||
                string.IsNullOrEmpty(newClient.Email) ||
                string.IsNullOrEmpty(newClient.Pesel) ||
                string.IsNullOrEmpty(newClient.Telephone))
            {
                return BadRequest();
            }
            
            var newClientId = await _clientsService.CreateClientAsync(newClient, cancellationToken);

            if (newClientId is null)
            {
                return NotFound();
            }
            
            return Created(string.Empty, new { IdClient = newClientId });
        }
    }
}