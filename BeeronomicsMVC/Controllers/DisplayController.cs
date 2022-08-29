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

        public IActionResult BigChart()
        {
            return View();
        }

        public async Task<IActionResult> Shuffle()
        {
            ServiceResponse<List<Drink>> response = await _drinkService.GetAllDrinks();
            if (!response.Success || response.Data.Count == 0)
                return NotFound();

            return View(response.Data);
        }

        [HttpGet]
        public async Task<List<PurchaseHistory>> GetChartData(int id)
        {
            List<PurchaseHistory> PurchaseHistory = _drinkService.GetPurchaseHistoryForDrink(id);
            return PurchaseHistory;
        }

        [HttpGet]
        public async Task<Dictionary<string, List<decimal>>> GetBigChartData()
        {
            Dictionary<string, List<decimal>> purchaseHistories = await _drinkService.GetPurchaseHistories();
            return purchaseHistories;
        }
    }
}
