using Application.Common.CategoryDTOS;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : GenericController<Category, CategoryDTO, object, AddCategoryDTO, UpdateCategoryDTO>
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service) : base(service)
        {
            _service = service;
        }
    }
}
