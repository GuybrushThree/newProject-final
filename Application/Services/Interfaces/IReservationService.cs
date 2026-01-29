namespace API.Application;

public interface IReservationService
{
    public Task<ReservationResponse> CreateAsync(CreateReservationRequest dto);

    public Task<bool> CancelAsync(Guid id);

    public Task<IEnumerable<ReservationResponse>> GetAllActiveAsync();

    public Task<MonthlyReportResponse> GetMonthlyReportAsync(int month, int year);
}