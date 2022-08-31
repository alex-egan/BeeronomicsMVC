using System.Timers;

namespace BeeronomicsMVC.HostedServices
{
    public class TimedHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private List<DrinkTimer> _timers = new List<DrinkTimer>();
        public TimedHostedService(IServiceScopeFactory scopeFactory, IServiceProvider provider)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // keep looping until we get a cancellation request
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SetInitialPrices();
                    await SetTimers();
                    await Task.Delay(1000 * 120);
                    await InitiateCrash();
                    await Task.Delay(1000 * 20);
                    await EndCrash();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            await EndCrash();
        }

        private async Task SetInitialPrices()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ICrashService>();
                await context.SetInitialPrices();
            }
        }

        private async Task InitiateCrash()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ICrashService>();
                await context.InitiateCrash();
            }
        }

        public async Task DecreaseDrinkPrice(int id)
        {
            DisplayDrink drink = new DisplayDrink();
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IDrinkService>();
                ServiceResponse<DisplayDrink> response = await context.DecreaseDrinkPrice(id);
                drink = response.Data;
            }
            await StartNewTimer(drink.ID);
        }

        public async Task SetTimers()
        {
            List<Drink> drinks = new List<Drink>();

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IDrinkService>();
                ServiceResponse<List<Drink>> response = await context.GetAllDrinks();
                drinks = response.Data;
            }

            for (int x = 0; x < drinks.Count; x++)
            {
                Random rnd = new Random();
                int interval = rnd.Next(15,60);
                DrinkTimer drinkTimer = new DrinkTimer
                {
                    DrinkID = drinks[x].ID,
                    Timer = new System.Timers.Timer(1000 * interval)
                };

                drinkTimer.Timer.Elapsed += async (source, e) => await DrinkTimerElapsed(source, e, drinkTimer.DrinkID);
                drinkTimer.Timer.Start();

                if (_timers.Count < drinks.Count)
                {
                    _timers.Add(drinkTimer);
                }
                else
                {
                    _timers[x] = drinkTimer;
                }
            }
        }

        public async Task StopAllTimers()
        {
            List<Drink> drinks = new List<Drink>();

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IDrinkService>();
                ServiceResponse<List<Drink>> response = await context.GetAllDrinks();
                drinks = response.Data;
            }

            for (int x = 0; x < drinks.Count; x++)
            {
                if (_timers[x].Timer != null)
                {
                    _timers[x].Timer.Stop();
                    _timers[x].Timer = null;
                }
            }
        }

        public async Task DrinkTimerElapsed(Object source, ElapsedEventArgs e, int id)
        {
            _ = DecreaseDrinkPrice(id);
        }

        public async Task StartNewTimer(int drinkID)
        {
            DrinkTimer timer = _timers[drinkID - 1];

            if (timer.Timer != null)
            {
                timer.Timer.Stop();
                timer.Timer = null;
                _timers[drinkID - 1] = timer;
            }

            Crash currentCrash = new Crash();
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ICrashService>();
                currentCrash = await context.GetLatestCrashAsync();
            }

            if (currentCrash != null && currentCrash.IsActive == false)
            {
                Random rnd = new Random();
                int interval = rnd.Next(15, 60);
                timer.Timer = new System.Timers.Timer(1000 * interval);
                timer.Timer.Elapsed += async (source, e) => await DrinkTimerElapsed(source, e, drinkID);
                timer.Timer.Start();
                _timers[drinkID - 1] = timer;
            }
        }

        public async Task EndCrash()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ICrashService>();
                await context.EndCrash();
            }
        }
    }
}
