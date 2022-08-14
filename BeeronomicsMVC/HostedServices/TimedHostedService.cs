namespace BeeronomicsMVC.HostedServices
{
    public class TimedHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _provider;
        public TimedHostedService(IServiceScopeFactory scopeFactory, IServiceProvider provider)
        {
            _scopeFactory = scopeFactory;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // keep looping until we get a cancellation request
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    SetInitialPrices();
                    await Task.Delay(1000*20);
                    InitiateCrash();
                    await Task.Delay(1000*20);
                    //Crash Ends
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        private async void SetInitialPrices()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ICrashService>();
                await context.SetInitialPrices();
            }
        }

        private async void InitiateCrash()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ICrashService>();
                await context.InitiateCrash();
            }
        }

        public async Task DecreaseDrinkPrice(Drink drink)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IDrinkService>();
                ServiceResponse<DrinkSimple> response = await context.DecreaseDrinkPrice(drink.ID);
                Console.WriteLine("Success");
            }
        }

        public void Elapsed(object state)
        {
            Drink drink = (Drink)state;
            _ = DecreaseDrinkPrice(drink);
        }

        public Timer StartNewTimer(Drink drink)
        {
            if (drink.Timer != null)
            {
                drink.Timer.Change(Timeout.Infinite, Timeout.Infinite);
                drink.Timer = null;
            }

            Random rnd = new Random();
            int interval = rnd.Next(10, 20);
            Timer timer = new Timer(Elapsed, drink, (1000 * interval), Timeout.Infinite);
            return timer;
        }
    }
}
