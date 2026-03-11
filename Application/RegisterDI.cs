using Application.Behaviors;
using Application.Interfaces;
using Application.Interfaces.IAuth;
using Application.Mappings;
using Application.Services;
using Application.Services.Auth;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class RegisterDI
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMappingProfile).Assembly);

            services.AddMediatR(typeof(RegisterDI).Assembly);
            services.AddValidatorsFromAssembly(typeof(RegisterDI).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddScoped(typeof(IGenericService<,,,,>), typeof(GenericService<,,,,>));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<ICategoryService, CategoryService>();

            // New services
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
