using Application.Common.DiscountDTOS;
using Application.Common.Pagination;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] PagingDTO paging)
        {
            var result = await _discountService.GetAllAsync(paging);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _discountService.GetByCodeAsync(code);
            return result == null
                ? NotFound(new { success = false, message = "Discount code not found." })
                : Ok(new { success = true, data = result });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AddDiscountDTO dto)
        {
            var result = await _discountService.CreateAsync(dto);
            return Ok(new { success = true, message = "Discount created successfully", data = result });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiscountDTO dto)
        {
            var result = await _discountService.UpdateAsync(id, dto);
            return Ok(new { success = true, message = "Discount updated successfully", data = result });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _discountService.DeleteAsync(id);
            return Ok(new { success = true, message = "Discount deleted successfully" });
        }

        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> Apply([FromBody] ApplyDiscountDto dto)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!;
            var result = await _discountService.ApplyAsync(dto.OrderId, dto.Code, userId);
            return Ok(new { success = true, data = result });
        }
    }
}
