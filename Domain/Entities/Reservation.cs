namespace API.Domain;

public class Reservation
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string IdFoodtruck { get; set; } = string.Empty;
    public TruckSize Size { get; set; }
    public decimal Price { get; set; }
    public bool IsCancelled { get; set; }

}