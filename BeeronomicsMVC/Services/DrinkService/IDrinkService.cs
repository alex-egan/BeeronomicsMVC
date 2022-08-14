namespace BeeronomicsMVC.Services.DrinkService
{
    public interface IDrinkService
    {
        Task<ServiceResponse<List<Drink>>> GetAllDrinks();
        Task<ServiceResponse<List<Drink>>> GetActiveDrinks();
        Task<ServiceResponse<DisplayDrink>> GetDrink(int id);
        Task<ServiceResponse<DisplayDrink>> UpdateDrink(DisplayDrink drink);
        Task<ServiceResponse<DisplayDrink>> IncreaseDrinkPrice(int id);
        Task<ServiceResponse<DisplayDrink>> DecreaseDrinkPrice(int id);
        Task<ServiceResponse<bool>> ToggleActiveStatus(int id);
        Task<Drink> GetDrinkFromDBByID(int id);
        DisplayDrink CreateDisplayDrinkObject(Drink drink);
        Task<List<Drink>> GetAllDrinksFromDB();
        Task<List<Drink>> GetActiveDrinksFromDB();
        void AddPurchaseHistoryForDrink(Drink drink);
        List<PurchaseHistory> GetPurchaseHistoryForDrink(int id);

    }
}
