namespace BeeronomicsMVC.Services.DisplayService
{
    public class DisplayService : IDisplayService
    {
        private readonly IJSRuntime _jSRuntime;

        public DisplayService(IJSRuntime jsRuntime)
        {
            _jSRuntime = jsRuntime;
        }

        public async Task UpdateDrinkPrice(Drink drink)
        {
            await _jSRuntime.InvokeAsync<Drink>("updateDrinkPrice", drink);
        }
    }
}
