using Application.Common.CartDTOS;
using Application.Common.OrderDTOS;
using Application.Features.Orders.Commands;
using Application.Features.Orders.Queries;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICartService _cartService;

        public OrdersController(IMediator mediator, ICartService cartService)
        {
            _mediator = mediator;
            _cartService = cartService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private bool IsAdmin => User.IsInRole("Admin");

        // ─── Cart ────────────────────────────────────────────────────────────

        [HttpGet("cart")]
        public async Task<IActionResult> GetCart()
        {
            var result = await _cartService.GetCartAsync(UserId);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("cart/items")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemAddDto dto)
        {
            var result = await _cartService.AddItemAsync(UserId, dto);
            return Ok(new { success = true, data = result });
        }

        [HttpPut("cart/items/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(string itemId, [FromBody] CartItemUpdateDto dto)
        {
            var result = await _cartService.UpdateItemAsync(UserId, itemId, dto);
            return Ok(new { success = true, data = result });
        }

        [HttpDelete("cart/items/{itemId}")]
        public async Task<IActionResult> RemoveCartItem(string itemId)
        {
            var result = await _cartService.RemoveItemAsync(UserId, itemId);
            return Ok(new { success = true, data = result });
        }

        [HttpPost("cart/clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync(UserId);
            return Ok(new { success = true, data = result });
        }

        // ─── Orders ──────────────────────────────────────────────────────────

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var result = await _mediator.Send(new CreateOrderCommand { UserId = UserId, Order = dto });
            return Ok(new { success = true, message = "Order created successfully", data = result });
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetOrdersQuery query)
        {
            query.UserId = UserId;
            query.IsAdmin = IsAdmin;
            var result = await _mediator.Send(query);
            return Ok(new { success = true, message = "Orders fetched successfully", data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetOrderDetailsQuery { Id = id, UserId = UserId, IsAdmin = IsAdmin });
            return Ok(new { success = true, message = "Order fetched successfully", data = result });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _mediator.Send(new UpdateOrderStatusCommand
            {
                Id = id,
                Status = dto.Status,
                Notes = dto.Notes,
                ChangedBy = UserId
            });
            return Ok(new { success = true, message = "Order status updated", data = result });
        }
    }
}
