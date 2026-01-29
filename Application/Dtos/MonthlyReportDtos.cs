namespace API.Application;

public record MonthlyReportResponse(
    int Month,
    int Year,
    IEnumerable<ReservationHistoryDto> History
)
{
    public decimal TotalRevenue => History.Where(h => !h.IsCancelled).Sum(h => h.Cost);
}

public record ReservationHistoryDto(
    string IdFoodTruck,
    DateTime ReservationDate,
    bool IsCancelled,
    decimal Cost
);