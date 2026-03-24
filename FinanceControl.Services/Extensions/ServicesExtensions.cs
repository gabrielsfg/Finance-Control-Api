using FinanceControl.Domain.Interfaces.Service;
using FinanceControl.Domain.Interfaces.Services;
using FinanceControl.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceControl.Services.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IAreaCategoryService, AreaCategoryService>();
            services.AddScoped<IBudgetSubCategoryAllocationService, BudgetSubCategoryAllocationService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IWishlistService, WishlistService>();
            // IExchangeRateService is registered via AddHttpClient in Program.cs

            return services;
        }
    }
}
