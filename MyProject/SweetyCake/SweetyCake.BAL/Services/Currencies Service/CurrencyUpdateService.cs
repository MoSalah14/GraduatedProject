using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OutbornE_commerce.BAL.Repositories.Currencies;
using OutbornE_commerce.BAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL
{
    public class CurrencyUpdateService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;  // Used to create scope for services
        private Timer _timer;

        public CurrencyUpdateService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Called when the background service starts
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Timer will trigger every 2 hours (7200 seconds)
            _timer = new Timer(UpdateCurrencies, null, TimeSpan.Zero, TimeSpan.FromHours(2));
            return Task.CompletedTask;
        }

        // The logic to update the currencies
        private async void UpdateCurrencies(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    // Use DI By ServiceProvider
                    var currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();
                    var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

                    
                    var GetValue = await currencyService.UpdateCurrecies();

                    
                    await currencyRepository.UpdateNewCurrencies(GetValue.Rates);
                }
                catch (Exception ex)
                {
                    // Add logging or error handling here
                    Console.WriteLine($"Error occurred while updating currencies: {ex.Message}");
                }
            }
        }

        // Called when the background service stops
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);  // Stop the timer
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();  // Clean up timer resources
        }
    }

}
