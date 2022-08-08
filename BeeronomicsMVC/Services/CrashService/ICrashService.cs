namespace BeeronomicsMVC.Services.CrashService
{
    public interface ICrashService
    {
        Task InitiateCrash();
        Task StopCrash();
        Task SetInitialPrices();
    }
}
