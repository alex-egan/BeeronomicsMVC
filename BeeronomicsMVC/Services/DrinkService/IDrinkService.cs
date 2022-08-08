namespace BeeronomicsMVC.Services.DrinkService
{
    public interface IDrinkService
    {
        Task<ServiceResponse<List<Drink>>> GetAllDrinks();
        Task<ServiceResponse<List<Drink>>> GetActiveDrinks();
        Task<ServiceResponse<DrinkSimple>> GetDrink(int id);
        Task<ServiceResponse<DrinkSimple>> UpdateDrink(DrinkSimple drink);
        Task<ServiceResponse<DrinkSimple>> PurchaseDrink(int id);
        Task<ServiceResponse<bool>> ToggleActiveStatus(int id);

    }
}
