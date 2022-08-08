using Microsoft.AspNetCore.Mvc;

namespace BeeronomicsMVC.Controllers
{
    public class DisplayController : Controller
    {
        private readonly IDrinkService _drinkService;
        public DisplayController(IDrinkService drinkService)
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
    }
}
