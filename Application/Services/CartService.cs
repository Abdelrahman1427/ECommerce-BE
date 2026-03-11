using Application.Common.CartDTOS;
using Application.Interfaces;
using Domain.Entities;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private static readonly Dictionary<string, List<CartItemInternal>> _carts = new();
        private static readonly object _lock = new();
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public Task<CartDto> GetCartAsync(string userId)
            => Task.FromResult(BuildCartDto(userId, GetItems(userId)));

        public async Task<CartDto> AddItemAsync(string userId, CartItemAddDto dto)
        {
            var product = await _unitOfWork.Repository<Product>()
                .GetQueryable().FirstOrDefaultAsync(p => p.Id == dto.ProductId)
                ?? throw new KeyNotFoundException($"Product {dto.ProductId} not found.");

            if (!product.IsActive)
                throw new InvalidOperationException("Product is not available.");
            if (product.StockQuantity < dto.Quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {product.StockQuantity}");

            lock (_lock)
            {
                var items = GetItems(userId);
                var existing = items.FirstOrDefault(i => i.ProductId == dto.ProductId);
                if (existing != null)
                    existing.Quantity += dto.Quantity;
                else
                    items.Add(new CartItemInternal
                    {
                        ItemId = Guid.NewGuid().ToString(),
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = dto.Quantity
                    });
            }
            return BuildCartDto(userId, GetItems(userId));
        }

        public Task<CartDto> UpdateItemAsync(string userId, string itemId, CartItemUpdateDto dto)
        {
            lock (_lock)
            {
                var item = GetItems(userId).FirstOrDefault(i => i.ItemId == itemId)
                    ?? throw new KeyNotFoundException("Cart item not found.");
                item.Quantity = dto.Quantity;
            }
            return Task.FromResult(BuildCartDto(userId, GetItems(userId)));
        }

        public Task<CartDto> RemoveItemAsync(string userId, string itemId)
        {
            lock (_lock) { GetItems(userId).RemoveAll(i => i.ItemId == itemId); }
            return Task.FromResult(BuildCartDto(userId, GetItems(userId)));
        }

        public Task<CartDto> ClearCartAsync(string userId)
        {
            lock (_lock) { if (_carts.ContainsKey(userId)) _carts[userId].Clear(); }
            return Task.FromResult(BuildCartDto(userId, new List<CartItemInternal>()));
        }

        // Called by OrderService
        public static List<CartItemInternal> GetCartItems(string userId)
        {
            lock (_lock) return GetItems(userId).ToList();
        }

        public static void ClearUserCart(string userId)
        {
            lock (_lock) { if (_carts.ContainsKey(userId)) _carts[userId].Clear(); }
        }

        private static List<CartItemInternal> GetItems(string userId)
        {
            if (!_carts.ContainsKey(userId)) _carts[userId] = new();
            return _carts[userId];
        }

        private static CartDto BuildCartDto(string userId, List<CartItemInternal> items)
        {
            var subTotal = items.Sum(i => i.UnitPrice * i.Quantity);
            return new CartDto
            {
                UserId = userId,
                Items = items.Select(i => new CartItemDto
                {
                    ItemId = i.ItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.UnitPrice * i.Quantity
                }).ToList(),
                SubTotal = subTotal,
                GrandTotal = subTotal,
                UpdatedAt = DateTimeOffset.UtcNow
            };
        }

        public class CartItemInternal
        {
            public string ItemId { get; set; } = string.Empty;
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
        }
    }
}
