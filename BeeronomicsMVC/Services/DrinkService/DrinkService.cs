using Microsoft.EntityFrameworkCore;

namespace BeeronomicsMVC.Services.DrinkService
{
    public class DrinkService : IDrinkService
    {
        private readonly BeeronomicsDBContext _context;
        public DrinkService(BeeronomicsDBContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<bool>> ToggleActiveStatus(int id)
        {
            Drink drink = await _context.Drink
                .Include(d => d.DrinkPrices)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (drink == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false
                };
            }

            drink.Active = !drink.Active;

            _context.Drink.Update(drink);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Success = true,
                Data = drink.Active
            };
        }

        public async Task<ServiceResponse<DrinkSimple>> GetDrink(int id)
        {
            Drink drink = await _context.Drink
                .Include(d => d.DrinkPrices)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (drink == null)
            {
                return new ServiceResponse<DrinkSimple>
                {
                    Success = false,
                    Message = "Could not find drinks."
                };
            }

            DrinkSimple drinkSimple = new DrinkSimple
            {
                ID = drink.ID,
                Name = drink.Name,
                Symbol = drink.Symbol,
                Photo = drink.Photo,
                Description = drink.Description,
                Category = drink.Category,
                AddedBy = drink.AddedBy,
                ActivePrice = drink.DrinkPrices.ActivePrice,
                MaxPrice = drink.DrinkPrices.MaxPrice,
                MinPrice = drink.DrinkPrices.MinPrice
            };

            return new ServiceResponse<DrinkSimple>
            {
                Success = true,
                Data = drinkSimple
            };
        }

        public async Task<ServiceResponse<List<Drink>>> GetAllDrinks()
        {
            List<Drink> drinks = await _context
                .Drink
                .Include(d => d.DrinkPrices)
                .ToListAsync();

            if (drinks == null || drinks.Count == 0)
            {
                return new ServiceResponse<List<Drink>>
                {
                    Success = false,
                    Message = "Could not find drinks."
                };
            }

            return new ServiceResponse<List<Drink>>
            {
                Success = true,
                Data = drinks
            };
        }

        public async Task<ServiceResponse<List<Drink>>> GetActiveDrinks()
        {
            List<Drink> drinks = await _context
                .Drink
                .Include(d => d.DrinkPrices)
                .Where(d => d.Active == true)
                .ToListAsync();

            if (drinks == null || drinks.Count == 0)
            {
                return new ServiceResponse<List<Drink>>
                {
                    Success = false,
                    Message = "Could not find drinks."
                };
            }

            return new ServiceResponse<List<Drink>>
            {
                Success = true,
                Data = drinks
            };
        }

        public async Task<ServiceResponse<DrinkSimple>> PurchaseDrink(int id)
        {
            Drink drink = await _context.Drink.Include(d => d.DrinkPrices).FirstOrDefaultAsync(d => d.ID == id);
            if (drink == null)
            {
                return new ServiceResponse<DrinkSimple>
                {
                    Data = null,
                    Success = false
                };
            }

            drink.DrinkPrices.ActivePrice += decimal.Parse("0.20");
            drink.DrinkPrices.PriceLastIncreased = true;
            _context.Drink.Update(drink);
            await _context.SaveChangesAsync();

            Drink updatedDrink = await _context.Drink.Include(d => d.DrinkPrices).FirstOrDefaultAsync(d => d.ID == drink.ID);

            DrinkSimple updatedSimple = new DrinkSimple
            {
                ID = updatedDrink.ID,
                Name = updatedDrink.Name,
                Symbol = updatedDrink.Symbol,
                Description = updatedDrink.Description,
                Category = updatedDrink.Category,
                AddedBy = updatedDrink.AddedBy,
                ActivePrice = updatedDrink.DrinkPrices.ActivePrice,
                MaxPrice = updatedDrink.DrinkPrices.MaxPrice,
                MinPrice = updatedDrink.DrinkPrices.MinPrice,
                PriceLastIncreased = updatedDrink.DrinkPrices.PriceLastIncreased
            };

            return new ServiceResponse<DrinkSimple>
            {
                Data = updatedSimple,
                Success = true
            };
        }

        public async Task<ServiceResponse<DrinkSimple>> UpdateDrink(DrinkSimple updatedDrink)
        {
            Drink drink = await _context.Drink.Include(d => d.DrinkPrices).FirstOrDefaultAsync(d => d.ID == updatedDrink.ID);

            if (drink == null)
            {
                return new ServiceResponse<DrinkSimple>
                {
                    Data = null,
                    Success = false
                };
            }

            drink.Name = updatedDrink.Name;
            drink.Symbol = updatedDrink.Symbol;
            drink.Description = updatedDrink.Description;
            drink.Category = updatedDrink.Category;
            drink.Photo = updatedDrink.Photo;
            drink.DrinkPrices.ActivePrice = updatedDrink.ActivePrice;
            drink.DrinkPrices.MaxPrice = updatedDrink.MaxPrice;
            drink.DrinkPrices.MinPrice = updatedDrink.MinPrice;

            _context.Drink.Update(drink);
            await _context.SaveChangesAsync();

            return new ServiceResponse<DrinkSimple>
            {
                Data = updatedDrink,
                Success = true
            };
        }
    }
}
