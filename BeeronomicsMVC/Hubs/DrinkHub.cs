namespace BeeronomicsMVC.Hubs
{
    public class DrinkHub : Hub
    {
        private readonly IDrinkService _drinkService;
        public DrinkHub(IDrinkService drinkService)
        {
            _drinkService = drinkService;
        }

        public async Task UpdateDrinkPrice(int id, bool increase)
        {
            try
            {
                ServiceResponse<DrinkSimple> response = new ServiceResponse<DrinkSimple>();
                if (increase)
                {
                    response = await _drinkService.IncreaseDrinkPrice(id);
                }
                else
                {
                    response = await _drinkService.DecreaseDrinkPrice(id);
                }

                if (response.Success)
                    await Clients.All.SendAsync("DrinkPriceUpdated", response.Data);
                
                
            }
            catch (Exception e)
            {
                Console.WriteLine();
            }
        }

        public async Task UpdateDrink(DrinkSimple drink)
        {
            try
            {
                ServiceResponse<DrinkSimple> response = await _drinkService.UpdateDrink(drink);
                if (response.Success)
                    await Clients.All.SendAsync("DrinkUpdated", response.Data);
            }
            catch
            {
                Console.WriteLine();
            }
        }

        public async Task ToggleActiveStatus(int id)
        {
            try
            {
                ServiceResponse<bool> response = await _drinkService.ToggleActiveStatus(id);
                if (response.Success)
                    await Clients.All.SendAsync("ActiveStatusToggled", response.Data, id);
            }
            catch
            {
                Console.WriteLine();
            }
        }
    }
}
