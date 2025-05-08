namespace Tutorial8.Models.DTOs;

public class Client_Trip
{
    public int idClient { get; set; }
    public int idTrip { get; set; }
    public DateTime RegisteredAt { get; set; }
    public DateTime? PaymentDate { get; set; }
}