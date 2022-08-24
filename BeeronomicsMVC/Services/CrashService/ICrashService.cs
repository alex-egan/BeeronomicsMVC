namespace BeeronomicsMVC.Services.CrashService
{
    public interface ICrashService
    {
        Task InitiateCrash();
        Task SetInitialPrices();
        Task<bool> IsCrashActive();
        Task<bool> EndCrash();
        Task<Crash> GetLatestCrashAsync();
        Crash GetLatestCrash();
    }
}
