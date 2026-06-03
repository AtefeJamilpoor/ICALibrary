
using ServiceLayer;

namespace ICALibrary
{
    public class OverdueBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OverdueBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var checker = scope.ServiceProvider.GetRequiredService<OverdueCheckerService>();
                    await checker.CheckOverdueUsersAsync();
                }
                // اجرا هر 24 ساعت
               await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
