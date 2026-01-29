namespace API.Application;

using API.Domain;

public class ReservationService : IReservationService
{
    private readonly List<Reservation> _database = new();

    private readonly object _lock = new();

    public async Task<ReservationResponse> CreateAsync(CreateReservationRequest request)
    {
        var today = DateTime.Today;

        if (request.ReservationDate.Date < today.AddDays(-BusinessRules.MaxBookingAdvanceDays) && request.ReservationDate.Date < today)
        {
            throw new InvalidOperationException("La date doit être comprise entre aujourd'hui et J+7.");
        }

        lock (_lock)
        {
            int dayCapacity = (request.ReservationDate.DayOfWeek == DayOfWeek.Friday)
                ? BusinessRules.TotalCapacityUnits - BusinessRules.FridayUnitReduction
                : BusinessRules.TotalCapacityUnits;

            int occupiedUnits = _database
                .Where(r => r.Date.Date == request.ReservationDate.Date && !r.IsCancelled)
                .Sum(r => r.Size == TruckSize.Large ? BusinessRules.LargeTruckUnits : BusinessRules.SmallTruckUnits);

            int requestedUnits = request.Size == TruckSize.Large ? BusinessRules.LargeTruckUnits : BusinessRules.SmallTruckUnits;

            if (occupiedUnits + requestedUnits > dayCapacity)
            {
                throw new InvalidOperationException("Il n'y a pas assez de place.");
            }

            decimal price = (request.ReservationDate.Date == today)
                ? BusinessRules.SameDayPrice
                : BusinessRules.StandardPrice;

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                IdFoodtruck = request.IdFoodTruck,
                Date = request.ReservationDate.Date,
                Size = request.Size,
                Price = price,
                IsCancelled = false,
            };

            _database.Add(reservation);
            return MapToResponse(reservation);
        }
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        lock (_lock)
        {
            var reservation = _database.FirstOrDefault(r => r.Id == id);

            if (reservation == null || reservation.IsCancelled)
                return false;

            var refundDate = reservation.Date.AddDays(-2);
            var today = DateTime.Today;
            if (today > refundDate && today < reservation.Date)
            {
                reservation.Price = 0;
            }
            reservation.IsCancelled = true;
            return true;
        }
    }

    public async Task<IEnumerable<ReservationResponse>> GetAllActiveAsync()
    {
        lock (_lock)
        {
            return _database
                .Where(r => !r.IsCancelled)
                .Select(MapToResponse)
                .ToList();
        }
    }

    public async Task<MonthlyReportResponse> GetMonthlyReportAsync(int month, int year)
    {
        lock (_lock)
        {
            var history = _database
                .Where(r => r.Date.Month == month && r.Date.Year == year)
                .Select(r => new ReservationHistoryDto(
                    r.IdFoodtruck,
                    r.Date,
                    r.IsCancelled,
                    r.Price))
                .ToList();

            return new MonthlyReportResponse(month, year, history);
        }
    }

    private static ReservationResponse MapToResponse(Reservation r) =>
        new(r.Id, r.IdFoodtruck, r.Date, r.Size, r.Price, r.IsCancelled);
}