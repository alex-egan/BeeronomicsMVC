using Microsoft.AspNetCore.Mvc;

namespace BeeronomicsMVC.Controllers
{
    public class DrinksController : Controller
    {
        private readonly IDrinkService _drinkService;
        public DrinksController(IDrinkService drinkService)
        {
            _drinkService = drinkService;
        }
        public async Task<IActionResult> Index()
        {
            ServiceResponse<List<Drink>> response = await _drinkService.GetAllDrinks();

            if (!response.Success || response.Data.Count == 0)
                return NotFound();

            return View(response.Data);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == -1)
            {
                return View(new DisplayDrink());
            }
            else
            {
                ServiceResponse<DisplayDrink> response = await _drinkService.GetDrink(id);
                if (!response.Success || response.Data == null)
                {
                    return NotFound();
                }

                return View(response.Data);
            }
        }
    }
}
