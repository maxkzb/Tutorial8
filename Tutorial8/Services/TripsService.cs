using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    public async Task<List<TripDTO>> GetTrips()
{
    var trips = new Dictionary<int, TripDTO>();

    var query = @"
        SELECT 
            t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
            c.Name AS CountryName,
            cl.FirstName, cl.LastName
        FROM Trip t
        LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
        LEFT JOIN Country c ON ct.IdCountry = c.IdCountry
        LEFT JOIN Client_Trip clt ON t.IdTrip = clt.IdTrip
        LEFT JOIN Client cl ON clt.IdClient = cl.IdClient
        ORDER BY t.IdTrip;
    ";

    using var conn = new SqlConnection(_connectionString);
    await conn.OpenAsync();
    using var cmd = new SqlCommand(query, conn);
    using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        int tripId = reader.GetInt32(reader.GetOrdinal("IdTrip"));

        if (!trips.ContainsKey(tripId))
        {
            trips[tripId] = new TripDTO
            {
                Id = tripId,
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                Countries = new List<CountryDTO>(),
                Clients = new List<ClientDTO>()
            };
        }

        // Add country if present and not already added
        if (!reader.IsDBNull(reader.GetOrdinal("CountryName")))
        {
            string countryName = reader.GetString(reader.GetOrdinal("CountryName"));
            if (!trips[tripId].Countries.Any(c => c.Name == countryName))
            {
                trips[tripId].Countries.Add(new CountryDTO { Name = countryName });
            }
        }

        // Add client if present and not already added
        if (!reader.IsDBNull(reader.GetOrdinal("FirstName")) && !reader.IsDBNull(reader.GetOrdinal("LastName")))
        {
            string firstName = reader.GetString(reader.GetOrdinal("FirstName"));
            string lastName = reader.GetString(reader.GetOrdinal("LastName"));

            if (!trips[tripId].Clients.Any(cl => cl.FirstName == firstName && cl.LastName == lastName))
            {
                trips[tripId].Clients.Add(new ClientDTO { FirstName = firstName, LastName = lastName });
            }
        }
    }

    return trips.Values.ToList();
}

}