﻿using Microsoft.EntityFrameworkCore;
using System.Timers;

namespace BeeronomicsMVC.Services.DrinkService
{
    public class DrinkService : IDrinkService
    {
        private readonly BeeronomicsDBContext _context;
        private readonly IHubContext<DrinkHub> _drinkHub;
        private readonly TimedHostedService _timedHostedService;
        public DrinkService(BeeronomicsDBContext context, IHubContext<DrinkHub> drinkHub, TimedHostedService timedHostedService)
        {
            _context = context;
            _drinkHub = drinkHub;
            _timedHostedService = timedHostedService;
        }

        public async Task<ServiceResponse<bool>> ToggleActiveStatus(int id)
        {
            Drink drink = await GetDrinkFromDBByID(id);

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

        public async Task<Drink> GetDrinkFromDBByID(int id)
        {
            return await _context.Drink
                .Include(d => d.DrinkPrices)
                .FirstOrDefaultAsync(d => d.ID == id);
        }

        public async Task<ServiceResponse<DisplayDrink>> GetDrink(int id)
        {
            Drink drink = await GetDrinkFromDBByID(id);

            if (drink == null)
            {
                return new ServiceResponse<DisplayDrink>
                {
                    Success = false,
                    Message = "Could not find drinks."
                };
            }

            DisplayDrink drinkSimple = CreateDisplayDrinkObject(drink);

            return new ServiceResponse<DisplayDrink>
            {
                Success = true,
                Data = drinkSimple
            };
        }

        public DisplayDrink CreateDisplayDrinkObject(Drink drink)
        {
            return new DisplayDrink
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
                MinPrice = drink.DrinkPrices.MinPrice,
                PriceLastIncreased = drink.PriceLastIncreased
            };
        }

        public async Task<ServiceResponse<List<Drink>>> GetAllDrinks()
        {
            List<Drink> drinks = await GetAllDrinksFromDB();

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

        public async Task<ServiceResponse<List<Drink>>> GetAllDrinksForShuffle()
        {
            List<Drink> drinks = await _context.Drink
                .Include(d => d.DrinkPrices)
                .OrderByDescending(d => d.DrinkPrices.ActivePrice)
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

        public async Task<List<Drink>> GetAllDrinksFromDB()
        {
            return await _context
                .Drink
                .Include(d => d.DrinkPrices)
                .ToListAsync();
        }

        public async Task<ServiceResponse<List<Drink>>> GetActiveDrinks()
        {
            List<Drink> drinks = await GetActiveDrinksFromDB();

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

        public async Task<List<Drink>> GetActiveDrinksFromDB()
        {
            return await _context
                .Drink
                .Include(d => d.DrinkPrices)
                .Where(d => d.Active == true)
                .ToListAsync();
        }

        public async Task<ServiceResponse<DisplayDrink>> IncreaseDrinkPrice(int id)
        {
            Drink drink = await GetDrinkFromDBByID(id);
            if (drink == null)
            {
                return new ServiceResponse<DisplayDrink>
                {
                    Data = null,
                    Success = false
                };
            }

            //int x = drink.PurchaseCount - drink.TimerElapsedCount;
            //drink.DrinkPrices.ActivePrice = Convert.ToDecimal(Convert.ToDouble(drink.DrinkPrices.MaxPrice)/(1 + 3 * Math.Exp(Convert.ToDouble(-1 * drink.DrinkPrices.KVal * x))));

            drink.DrinkPrices.ActivePrice += decimal.Parse("0.20");
            if (drink.DrinkPrices.ActivePrice > drink.DrinkPrices.MaxPrice)
                drink.DrinkPrices.ActivePrice = drink.DrinkPrices.MaxPrice;
            drink.PriceLastIncreased = true;
            
            AddPurchaseHistoryForDrink(drink);
            _context.Drink.Update(drink);
            await _context.SaveChangesAsync();

            await _timedHostedService.StartNewTimer(drink.ID);

            DisplayDrink displayDrink = CreateDisplayDrinkObject(drink);

            await _drinkHub.Clients.All.SendAsync("DrinkPriceUpdated", displayDrink);

            return new ServiceResponse<DisplayDrink>
            {
                Data = displayDrink,
                Success = true
            };
        }

        public void AddPurchaseHistoryForDrink(Drink drink)
        {
            _context.PurchaseHistory.Add(new PurchaseHistory
            {
                Fk_Drink_ID = drink.ID,
                Purchases = 1,
                ActivePrice = drink.DrinkPrices.ActivePrice,
                TimeStamp = DateTime.Now,
            });
        }

        public async Task<ServiceResponse<DisplayDrink>> UpdateDrink(DisplayDrink updatedDrink)
        {
            Drink drink = await GetDrinkFromDBByID(updatedDrink.ID);

            if (drink == null)
            {
                return new ServiceResponse<DisplayDrink>
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

            return new ServiceResponse<DisplayDrink>
            {
                Data = updatedDrink,
                Success = true
            };
        }

        public async Task<ServiceResponse<DisplayDrink>> DecreaseDrinkPrice(int id)
        {
            Drink drink = await GetDrinkFromDBByID(id);
            if (drink == null)
            {
                return new ServiceResponse<DisplayDrink>
                {
                    Data = null,
                    Success = false
                };
            }

            drink.DrinkPrices.ActivePrice -= decimal.Parse("0.10");
            if (drink.DrinkPrices.ActivePrice < drink.DrinkPrices.MinPrice)
                drink.DrinkPrices.ActivePrice = drink.DrinkPrices.MinPrice;
            drink.PriceLastIncreased = false;

            AddPurchaseHistoryForDrink(drink);
            _context.Drink.Update(drink);
            await _context.SaveChangesAsync();

            DisplayDrink displayDrink = CreateDisplayDrinkObject(drink);

            await _drinkHub.Clients.All.SendAsync("DrinkPriceUpdated", displayDrink);

            return new ServiceResponse<DisplayDrink>
            {
                Data = displayDrink,
                Success = true
            };
        }

        public List<PurchaseHistory> GetPurchaseHistoryForDrink(int id)
        {
            return _context.PurchaseHistory
                .Where(d => d.Fk_Drink_ID == id)
                .OrderByDescending(d => d.TimeStamp)
                .Take(20)
                .ToList();
        }

        public async Task<Drink> CreateDrink(DisplayDrink displayDrink)
        {
            Drink drink = new Drink
            {
                Symbol = displayDrink.Symbol,
                Name = displayDrink.Name,
                Description = displayDrink.Description,
                Category = displayDrink.Category,
                AddedBy = "afegan",
                DateAdded = DateTime.Now,
                UpdatedBy = "",
                DateUpdated = null,
                Active = true,
                Photo = displayDrink.Photo,
                PriceLastIncreased = true,
            };

            drink.DrinkPrices.ActivePrice = displayDrink.ActivePrice;
            drink.DrinkPrices.MaxPrice = displayDrink.MaxPrice;
            drink.DrinkPrices.MinPrice = displayDrink.MinPrice;
            drink.DrinkPrices.UpdatedOn = DateTime.Now;
            drink.DrinkPrices.PriceLastIncreased = true;

            _context.Drink.Add(drink);
            await _context.SaveChangesAsync();
            return drink;
        }

        public async Task<Dictionary<string, List<decimal>>> GetPurchaseHistories()
        {
            Dictionary<string, List<decimal>> purchaseHistories = new Dictionary<string, List<decimal>>();

            List<Drink> drinks = (await GetAllDrinks()).Data;

            drinks.ForEach(drink =>
            {
                List<decimal> activePrices = new List<decimal>();
                List<PurchaseHistory> purchaseHistory = GetPurchaseHistoryForDrink(drink.ID);
                purchaseHistory.ForEach(ph =>
                {
                    activePrices.Add(ph.ActivePrice);
                });

                activePrices.Reverse();
                purchaseHistories.Add(drink.Name, activePrices);
            });

            return purchaseHistories;
        }
    }
}
