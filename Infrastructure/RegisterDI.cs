using Application.Interfaces;
using Domain.Entities;
using Domain.IRepository;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class RegisterDI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ECommerceContext>((serviceProvider, opt) =>
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                opt.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(30);
                });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ECommerceContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<ApplicationUser>, Services.BCryptPasswordHasher>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // NotificationService (Application interface, Infrastructure implementation)
            services.AddScoped<INotificationService, NotificationService>();

            services.AddScoped<ICurrentTenantService, CurrentTenantService>();

            return services;
        }
    }
}
