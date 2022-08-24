using Microsoft.EntityFrameworkCore;

namespace BeeronomicsMVC.Services.CrashService
{
    public class CrashService : ICrashService
    {
        private readonly BeeronomicsDBContext _context;
        private readonly IHubContext<DrinkHub> _drinkHub;
        private readonly TimedHostedService _timedHostedService;
        private readonly IDrinkService _drinkService;
        public CrashService(BeeronomicsDBContext context,
                        IHubContext<DrinkHub> drinkHub,
                        TimedHostedService timedHostedService,
                        IDrinkService drinkService)
        {
            _context = context;
            _drinkHub = drinkHub;
            _timedHostedService = timedHostedService;
            _drinkService = drinkService;

        }
        public async Task InitiateCrash()
        {
            List<Drink> drinks = await _drinkService.GetAllDrinksFromDB();
            if (drinks.Count == 0)
                return;

            List<DisplayDrink> displayDrinks = new List<DisplayDrink>();

            drinks.ForEach(drink =>
            {
                drink.DrinkPrices.ActivePrice = drink.DrinkPrices.MinPrice;
                drink.PriceLastIncreased = false;
                _context.Drink.Update(drink);
                DisplayDrink displayDrink = _drinkService.CreateDisplayDrinkObject(drink);

                displayDrinks.Add(displayDrink);
            });

            _context.Crash.Add(new Crash
            {
                IsActive = true,
                StartTime = DateTime.Now
            });

            await _context.SaveChangesAsync();

            await _timedHostedService.StopAllTimers();
            await _drinkHub.Clients.All.SendAsync("CrashActionInitiated", displayDrinks);
        }

        public async Task<bool> EndCrash()
        {
            Crash latestCrash = await GetLatestCrashAsync();
            latestCrash.IsActive = false;
            latestCrash.EndTime = DateTime.Now;

            _context.Crash.Update(latestCrash);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsCrashActive()
        {
            Crash latestCrash = await GetLatestCrashAsync();

            return latestCrash.IsActive;
        }

        public async Task<Crash> GetLatestCrashAsync()
        {
            return await _context.Crash
                            .OrderByDescending(c => c.StartTime)
                            .FirstOrDefaultAsync();
        }

        public Crash GetLatestCrash()
        {
            Crash crash = _context.Crash
                            .OrderByDescending(c => c.StartTime)
                            .First();

            if (crash == null)
            {
                crash = new Crash();
            }

            return crash;
        }

        public async Task SetInitialPrices()
        {
            List<Drink> drinks = await _drinkService.GetAllDrinksFromDB();
            if (drinks.Count == 0)
                return;

            List<DisplayDrink> displayDrinks = new List<DisplayDrink>();

            drinks.ForEach(drink =>
            {
                drink.DrinkPrices.ActivePrice = (drink.DrinkPrices.MaxPrice + drink.DrinkPrices.MinPrice) / 2;
                drink.PriceLastIncreased = true;
                _context.Drink.Update(drink);

                DisplayDrink displayDrink = _drinkService.CreateDisplayDrinkObject(drink);

                displayDrinks.Add(displayDrink);
            });

            await _context.SaveChangesAsync();
            await _drinkHub.Clients.All.SendAsync("CrashActionInitiated", displayDrinks);
        }
    }
}
