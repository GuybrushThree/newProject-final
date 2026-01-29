namespace API.Domain;

public static class BusinessRules
{
    public const int UnitsPerSpot = 2;
    public const int TotalSpots = 7;
    public const int TotalCapacityUnits = TotalSpots * UnitsPerSpot;
    public const int FridaySpotReduction = 1;
    public const int FridayUnitReduction = FridaySpotReduction * UnitsPerSpot;
    public const int SmallTruckUnits = 1;


    // Un camion de 5 Um occupe 1 spot entier, soit 2 unités
    public const int LargeTruckUnits = 2;
    public const decimal StandardPrice = 20m;
    public const decimal SameDayPrice = 40m;
    public const int MaxBookingAdvanceDays = 7;
    public const int FreeCancellationLimitDays = 2;
}