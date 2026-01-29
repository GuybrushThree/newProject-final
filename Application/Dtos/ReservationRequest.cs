using API.Domain;
namespace API.Application;

public record CreateReservationRequest(string IdFoodTruck, DateTime ReservationDate, TruckSize Size);
