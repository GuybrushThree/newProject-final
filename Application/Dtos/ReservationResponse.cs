using API.Domain;
namespace API.Application;

public record ReservationResponse(Guid Id, string IdFoodTruck, DateTime ReservationDate, TruckSize Size, decimal Price, bool IsCancelled);