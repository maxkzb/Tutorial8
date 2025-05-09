using Tutorial8.Models;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<ClientTripDto>> getClientTripsByIdAsync(int id, CancellationToken cancellationToken);
    Task<int?> CreateClientAsync(Client newClient, CancellationToken cancellationToken);
    Task<string> RegisterClientToTripAsync(int clientId, int tripId, CancellationToken cancellationToken);
    Task<string> UnregisterClientFromTripAsync(int clientId, int tripId, CancellationToken cancellationToken);
}