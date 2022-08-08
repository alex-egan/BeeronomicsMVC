using Microsoft.EntityFrameworkCore;

namespace BeeronomicsMVC.Services.CrashService
{
    public class CrashService : ICrashService
    {
        private readonly BeeronomicsDBContext _context;
        private readonly IHubContext<DrinkHub> _drinkHub;
        public CrashService(BeeronomicsDBContext context, IHubContext<DrinkHub> drinkHub)
        {
            _context = context;
            _drinkHub = drinkHub;
        }
        public async Task InitiateCrash()
        {
            List<Drink> drinks = await _context.Drink
                .Include(d => d.DrinkPrices)
                .ToListAsync();
            if (drinks.Count == 0)
                return;

            List<DrinkSimple> drinksSimple = new List<DrinkSimple>();

            drinks.ForEach(drink =>
            {
                drink.DrinkPrices.ActivePrice = drink.DrinkPrices.MinPrice;
                drink.PriceLastIncreased = false;
                _context.Drink.Update(drink);
                DrinkSimple updatedSimple = new DrinkSimple
                {
                    ID = drink.ID,
                    Name = drink.Name,
                    Symbol = drink.Symbol,
                    Description = drink.Description,
                    Category = drink.Category,
                    AddedBy = drink.AddedBy,
                    ActivePrice = drink.DrinkPrices.ActivePrice,
                    MaxPrice = drink.DrinkPrices.MaxPrice,
                    MinPrice = drink.DrinkPrices.MinPrice,
                    PriceLastIncreased = drink.PriceLastIncreased
                };

                drinksSimple.Add(updatedSimple);
            });

            await _context.SaveChangesAsync();
            await _drinkHub.Clients.All.SendAsync("CrashActionInitiated", drinksSimple);
        }

        public async Task SetInitialPrices()
        {
            List<Drink> drinks = await _context.Drink
                .Include(d => d.DrinkPrices)
                .ToListAsync();
            if (drinks.Count == 0)
                return;

            List<DrinkSimple> drinksSimple = new List<DrinkSimple>();

            drinks.ForEach(drink =>
            {
                drink.DrinkPrices.ActivePrice = (drink.DrinkPrices.MaxPrice + drink.DrinkPrices.MinPrice) / 2;
                drink.PriceLastIncreased = true;
                _context.Drink.Update(drink);

                DrinkSimple updatedSimple = new DrinkSimple
                {
                    ID = drink.ID,
                    Name = drink.Name,
                    Symbol = drink.Symbol,
                    Description = drink.Description,
                    Category = drink.Category,
                    AddedBy = drink.AddedBy,
                    ActivePrice = drink.DrinkPrices.ActivePrice,
                    MaxPrice = drink.DrinkPrices.MaxPrice,
                    MinPrice = drink.DrinkPrices.MinPrice,
                    PriceLastIncreased = drink.PriceLastIncreased
                };

                drinksSimple.Add(updatedSimple);
            });

            await _context.SaveChangesAsync();
            await _drinkHub.Clients.All.SendAsync("CrashActionInitiated", drinksSimple);
        }

        public Task StopCrash()
        {
            throw new NotImplementedException();
        }
    }
}
