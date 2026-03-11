using Application.Common.DiscountDTOS;
using Application.Common.Pagination;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DiscountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<DiscountDto>> GetAllAsync(PagingDTO paging)
        {
            var query = _unitOfWork.Repository<Discount>().GetQueryable().OrderByDescending(d => d.Created);
            var paged = await query.ToPagedResponseAsync(paging);
            var dtos = paged.Items.Select(d => _mapper.Map<DiscountDto>(d)).ToList();
            return new PagedResponse<DiscountDto>(dtos, paged.TotalCount, paged.PageNumber, paged.PageSize);
        }

        public async Task<DiscountDto?> GetByCodeAsync(string code)
        {
            var d = await _unitOfWork.Repository<Discount>()
                .GetQueryable().FirstOrDefaultAsync(x => x.Code == code.ToUpper());
            return d == null ? null : _mapper.Map<DiscountDto>(d);
        }

        public async Task<DiscountDto> CreateAsync(AddDiscountDTO dto)
        {
            if (!Enum.TryParse<DiscountType>(dto.Type, true, out var type))
                throw new ArgumentException($"Invalid discount type: {dto.Type}");

            var exists = await _unitOfWork.Repository<Discount>()
                .GetQueryable().AnyAsync(d => d.Code == dto.Code.ToUpper());
            if (exists)
                throw new InvalidOperationException($"Discount code '{dto.Code}' already exists.");

            var entity = new Discount
            {
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                Type = type,
                Value = dto.Value,
                MaxUsage = dto.MaxUsage,
                MinimumOrderTotal = dto.MinimumOrderTotal,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };

            await _unitOfWork.Repository<Discount>().AddAsync(entity, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            return _mapper.Map<DiscountDto>(entity);
        }

        public async Task<DiscountDto> UpdateAsync(int id, UpdateDiscountDTO dto)
        {
            var entity = await _unitOfWork.Repository<Discount>().GetByIdAsync(id, CancellationToken.None)
                ?? throw new KeyNotFoundException($"Discount {id} not found.");

            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.Type != null && Enum.TryParse<DiscountType>(dto.Type, true, out var t)) entity.Type = t;
            if (dto.Value.HasValue) entity.Value = dto.Value.Value;
            if (dto.MaxUsage.HasValue) entity.MaxUsage = dto.MaxUsage;
            if (dto.MinimumOrderTotal.HasValue) entity.MinimumOrderTotal = dto.MinimumOrderTotal;
            if (dto.StartDate.HasValue) entity.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            await _unitOfWork.Repository<Discount>().UpdateAsync(entity, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            return _mapper.Map<DiscountDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Discount>().GetByIdAsync(id, CancellationToken.None)
                ?? throw new KeyNotFoundException($"Discount {id} not found.");
            await _unitOfWork.Repository<Discount>().RemoveAsync(entity, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<ApplyDiscountResultDto> ApplyAsync(int orderId, string code, string userId)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId, CancellationToken.None)
                ?? throw new KeyNotFoundException($"Order {orderId} not found.");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            var discount = await _unitOfWork.Repository<Discount>()
                .GetQueryable(false).FirstOrDefaultAsync(d => d.Code == code.ToUpper());

            if (discount == null || !discount.IsActive)
                return new ApplyDiscountResultDto { Applied = false, Message = "Invalid or inactive discount code." };

            if (discount.EndDate.HasValue && discount.EndDate < DateTimeOffset.UtcNow)
                return new ApplyDiscountResultDto { Applied = false, Message = "Discount code has expired." };

            if (discount.MaxUsage.HasValue && discount.UsedCount >= discount.MaxUsage)
                return new ApplyDiscountResultDto { Applied = false, Message = "Discount usage limit reached." };

            decimal discountAmount = discount.Type switch
            {
                DiscountType.Percentage => Math.Round(order.TotalAmount * discount.Value / 100, 2),
                DiscountType.FixedAmount => Math.Min(discount.Value, order.TotalAmount),
                DiscountType.FreeShipping => 0,
                _ => 0
            };

            order.Discount = discountAmount;
            order.FinalAmount = order.TotalAmount - discountAmount;
            order.DiscountCode = discount.Code;
            discount.UsedCount++;

            await _unitOfWork.Repository<Order>().UpdateAsync(order, CancellationToken.None);
            await _unitOfWork.Repository<Discount>().UpdateAsync(discount, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            return new ApplyDiscountResultDto
            {
                Applied = true,
                DiscountAmount = discountAmount,
                NewTotal = order.FinalAmount,
                Message = $"Discount applied: -{discountAmount:C}"
            };
        }
    }
}
