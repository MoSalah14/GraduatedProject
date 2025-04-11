using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OutbornE_commerce.BAL.Repositories.DiscountCategoryRepo;
using OutbornE_commerce.BAL.Repositories.ProductSizes;

public class DiscountCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DiscountCleanupService> _logger;

    public DiscountCleanupService(IServiceProvider serviceProvider, ILogger<DiscountCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeSpan delayInterval = TimeSpan.FromMinutes(2);
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var discountCategoryRepository = scope.ServiceProvider.GetRequiredService<IDiscountCategoryRepository>();
                var productSizeRepository = scope.ServiceProvider.GetRequiredService<IProductSizeRepository>();

                try
                {
                    var allDiscounts = await discountCategoryRepository.FindAllAsync(null);

                    foreach (var discount in allDiscounts)
                    {
                        if (discount.EndDate < discount.StartDate && !discount.IsActive)
                        {
                            // Invalid date range: Activate and apply the discount
                            discount.IsActive = true;
                            await productSizeRepository.ApplyDiscountToCategory(discount.Id);
                            discountCategoryRepository.Update(discount);
                        }
                        else if (discount.EndDate <= DateTime.UtcNow && discount.IsActive)
                        {
                            // Discount has expired: Reset it
                            bool resetSuccess = await productSizeRepository.ResetDiscountsByCategoryId(discount.Id);
                            if (resetSuccess)
                            {
                                discount.IsActive = false;
                                discountCategoryRepository.Update(discount);
                            }
                        }
                    }

                    await discountCategoryRepository.SaveAsync(stoppingToken);
                    _logger.LogInformation("Discounts processed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during discount processing.");
                }
            }

            await Task.Delay(delayInterval, stoppingToken);
        }
    }

}
