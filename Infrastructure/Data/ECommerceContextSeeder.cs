using Domain.Entities;
using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public class ECommerceContextSeeder
    {
        public static async Task SeedAsync(ECommerceContext context, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ── Roles ──────────────────────────────────────────────────────
            foreach (var role in new[] { "Admin", "User" })
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // ── Admin ──────────────────────────────────────────────────────
            var adminEmail = "admin@ecommerce.com";
            ApplicationUser? adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    FullName = "System Admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // ── Test User ──────────────────────────────────────────────────
            var userEmail = "user@ecommerce.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var testUser = new ApplicationUser
                {
                    UserName = userEmail,
                    FullName = "Ahmed Mohamed",
                    Email = userEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(testUser, "User@123456");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(testUser, "User");
            }

            // ── Skip if already seeded ─────────────────────────────────────
            if (await context.Categories.AnyAsync()) return;

            var now = DateTimeOffset.UtcNow;

            // ═══════════════════════════════════════════════════════════════
            // CATEGORIES
            // ═══════════════════════════════════════════════════════════════
            var categories = new List<Category>
            {
                new() { Name = "إلكترونيات",  Description = "موبايلات، لاب توب، وأجهزة إلكترونية",  IsActive = true, Created = now, LastModified = now },
                new() { Name = "ملابس",        Description = "ملابس رجالي وحريمي وأطفال",             IsActive = true, Created = now, LastModified = now },
                new() { Name = "مطبخ ومنزل",  Description = "أجهزة مطبخ وأدوات منزلية",              IsActive = true, Created = now, LastModified = now },
                new() { Name = "كتب وتعليم",   Description = "كتب عربية وأجنبية وكورسات",             IsActive = true, Created = now, LastModified = now },
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            int elecId = categories[0].Id;
            int clothId = categories[1].Id;
            int homeId = categories[2].Id;
            int bookId = categories[3].Id;

            // ═══════════════════════════════════════════════════════════════
            // PRODUCTS
            // ═══════════════════════════════════════════════════════════════
            var products = new List<Product>
            {
                // ── إلكترونيات ────────────────────────────────────────────
                new()
                {
                    Name          = "موبايل سامسونج A55",
                    Description   = "شاشة Super AMOLED 6.6 بوصة، كاميرا 50MP، بطارية 5000mAh، رام 8GB. الأفضل في فئة المتوسطة.",
                    Price         = 12999m,
                    StockQuantity = 40,
                    CategoryId    = elecId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=600",
                    Created       = now, LastModified = now
                },
                new()
                {
                    Name          = "لاب توب لينوفو IdeaPad",
                    Description   = "معالج Intel Core i5 الجيل الثالث عشر، رام 8GB، SSD 512GB، شاشة 15.6 بوصة FHD.",
                    Price         = 32500m,
                    StockQuantity = 20,
                    CategoryId    = elecId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=600",
                    Created       = now, LastModified = now
                },
                new()
                {
                    Name          = "سماعة JBL Tune 520BT",
                    Description   = "سماعة بلوتوث لاسلكية، بطارية 57 ساعة، جودة صوت عالية، خفيفة الوزن.",
                    Price         = 2199m,
                    StockQuantity = 60,
                    CategoryId    = elecId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600",
                    Created       = now, LastModified = now
                },

                // ── ملابس ─────────────────────────────────────────────────
                new()
                {
                    Name          = "جلابية رجالي فاخرة",
                    Description   = "جلابية قطن مصري 100%، تفصيل أنيق، مناسبة للمناسبات والسهرات، متوفرة بعدة ألوان.",
                    Price         = 450m,
                    StockQuantity = 80,
                    CategoryId    = clothId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=600",
                    Created       = now, LastModified = now
                },
                new()
                {
                    Name          = "فستان حريمي كاجوال",
                    Description   = "فستان ميدي قطن ناعم، تصميم بسيط وعصري، مريح للاستخدام اليومي. مقاسات S حتى XXL.",
                    Price         = 380m,
                    StockQuantity = 100,
                    CategoryId    = clothId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=600",
                    Created       = now, LastModified = now
                },

                // ── مطبخ ومنزل ────────────────────────────────────────────
                new()
                {
                    Name          = "طاجن فخار أسواني",
                    Description   = "طاجن فخار أصيل مصنوع يدوياً في أسوان، مثالي للطهي البطيء والأكلات الشعبية المصرية.",
                    Price         = 120m,
                    StockQuantity = 150,
                    CategoryId    = homeId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=600",
                    Created       = now, LastModified = now
                },
                new()
                {
                    Name          = "ماكينة قهوة نيسبريسو",
                    Description   = "ماكينة كبسولات نيسبريسو Essenza Mini، سريعة التسخين، مدمجة وأنيقة.",
                    Price         = 3800m,
                    StockQuantity = 30,
                    CategoryId    = homeId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1511920170033-f8396924c348?w=600",
                    Created       = now, LastModified = now
                },

                // ── كتب وتعليم ────────────────────────────────────────────
                new()
                {
                    Name          = "رواية عزازيل - يوسف زيدان",
                    Description   = "رواية تاريخية حائزة على جائزة البوكر العربية، تدور في مصر وسوريا في القرن الخامس الميلادي.",
                    Price         = 85m,
                    StockQuantity = 200,
                    CategoryId    = bookId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=600",
                    Created       = now, LastModified = now
                },
                new()
                {
                    Name          = "كتاب Clean Code بالعربي",
                    Description   = "النسخة العربية المترجمة من كتاب Clean Code لروبرت مارتن. مرجع أساسي لكل مطور.",
                    Price         = 150m,
                    StockQuantity = 120,
                    CategoryId    = bookId,
                    IsActive      = true,
                    ImageUrls     = "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=600",
                    Created       = now, LastModified = now
                },
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // ═══════════════════════════════════════════════════════════════
            // DISCOUNTS
            // ═══════════════════════════════════════════════════════════════
            var discounts = new List<Discount>
            {
                new()
                {
                    Code              = "AHLAN10",
                    Description       = "خصم 10% على أول طلب",
                    Type              = DiscountType.Percentage,
                    Value             = 10m,
                    MaxUsage          = 500,
                    UsedCount         = 0,
                    MinimumOrderTotal = 200m,
                    StartDate         = DateTime.UtcNow,
                    EndDate           = DateTime.UtcNow.AddDays(365),
                    IsActive          = true,
                    Created           = now, LastModified = now
                },
                new()
                {
                    Code              = "KHOSOM50",
                    Description       = "خصم 50 جنيه على الطلبات فوق 500 جنيه",
                    Type              = DiscountType.FixedAmount,
                    Value             = 50m,
                    MaxUsage          = 200,
                    UsedCount         = 0,
                    MinimumOrderTotal = 500m,
                    StartDate         = DateTime.UtcNow,
                    EndDate           = DateTime.UtcNow.AddDays(60),
                    IsActive          = true,
                    Created           = now, LastModified = now
                },
                new()
                {
                    Code              = "SHIPPING",
                    Description       = "شحن مجاني على جميع الطلبات",
                    Type              = DiscountType.FreeShipping,
                    Value             = 0m,
                    MaxUsage          = null,
                    UsedCount         = 0,
                    MinimumOrderTotal = null,
                    StartDate         = DateTime.UtcNow,
                    EndDate           = null,
                    IsActive          = true,
                    Created           = now, LastModified = now
                },
            };

            await context.Discounts.AddRangeAsync(discounts);
            await context.SaveChangesAsync();
        }
    }
}