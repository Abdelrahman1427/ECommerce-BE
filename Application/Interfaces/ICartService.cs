using Application.Common.CartDTOS;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> AddItemAsync(string userId, CartItemAddDto dto);
        Task<CartDto> UpdateItemAsync(string userId, string itemId, CartItemUpdateDto dto);
        Task<CartDto> RemoveItemAsync(string userId, string itemId);
        Task<CartDto> ClearCartAsync(string userId);
    }
}
