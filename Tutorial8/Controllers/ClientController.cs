using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/clients/[id]/trips")]
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
    }
}