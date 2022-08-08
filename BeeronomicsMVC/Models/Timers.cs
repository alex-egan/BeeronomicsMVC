using Microsoft.EntityFrameworkCore;
using System.Timers;

namespace BeeronomicsMVC.Models
{
    public class Timers : IHostedService
    {
        private readonly BeeronomicsDBContext _context;
        public Timers(BeeronomicsDBContext context)
        {
            _context = context;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            List<Drink> drinks = await _context.Drink
                .ToListAsync();
            drinks.ForEach(drink =>
            {
                //drink.Timer = SetTimer(drink.ID);
            });
        }

        private System.Timers.Timer SetTimer(int id)
        {
            System.Timers.Timer timer = new System.Timers.Timer(10000);
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) => TimerExpired(sender, e, id);
            timer.Enabled = true;

            return timer;
        }

        private async void TimerExpired(Object sender, ElapsedEventArgs e, int id)
        {
            Drink drink = await _context.Drink
                .Include(d => d.DrinkPrices)
                .FirstOrDefaultAsync(d => d.ID == id);
            if (drink == null)
            {
                return;
            }

            drink.DrinkPrices.ActivePrice -= decimal.Parse("0.20");
            drink.PriceLastIncreased = false;

            _context.Drink.Update(drink);
            await _context.SaveChangesAsync();

            SetTimer(id);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
