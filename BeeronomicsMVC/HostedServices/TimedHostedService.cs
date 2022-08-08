namespace BeeronomicsMVC.HostedServices
{
    public class TimedHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public TimedHostedService(IServiceScopeFactory scopeFactory)
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
                    SetInitialPrices();
                    await Task.Delay(1000*30);
                    InitiateCrash();
                    await Task.Delay(1000*10);
                    //Crash Ends
                }
                catch (OperationCanceledException)
                {
                    // catch the cancellation exception
                    // to stop execution
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
    }
}
