using System.Linq.Expressions;
using Application.Common.Pagination;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public abstract class GenericController<TEntity, TGetDTO, TFilter, TAddDTO, TUpdateDTO> : ControllerBase
        where TEntity : class
        where TGetDTO : class
        where TFilter : class?
        where TAddDTO : class
        where TUpdateDTO : class
    {
        protected readonly IGenericService<TEntity, TGetDTO, TFilter, TAddDTO, TUpdateDTO> _service;
        protected virtual Expression<Func<TEntity, object>>[] includes => Array.Empty<Expression<Func<TEntity, object>>>();

        public GenericController(
            IGenericService<TEntity, TGetDTO, TFilter, TAddDTO, TUpdateDTO> service
           )
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TGetDTO>>> GetAll(CancellationToken ct)
        {
            var result = await _service.GetAllAsync( ct);
            return Ok(result);
        }
        [HttpPost("FilteredPaged")]
        public virtual async Task<ActionResult<PagedResponse<TGetDTO>>> GetFilteredPaged([FromBody] PagingDTO<TFilter>? paging, CancellationToken ct)
        {
            var result = await _service.GetFilteredPagedAsync(paging, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TGetDTO>> GetById(int id, CancellationToken ct)
        {
            var entity = await _service.GetByIdAsync(id, ct);
            if (entity == null)
            {
                return NotFound(new { Message = $"Entity with ID {id} not found." });
            }

            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<ActionResult> Create([FromBody] TAddDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.CreateAsync(dto, ct);
            return Created();
        }


        [HttpPut("{id}")]
        public virtual async Task<ActionResult> Update(int id, [FromBody] TUpdateDTO dto, CancellationToken ct) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _service.GetByIdAsync(id, ct);
            if (existing == null)
            {
                return NotFound(new { Message = $"Entity with ID {id} not found." });
            }

            await _service.UpdateAsync(id, dto, ct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            var existing = await _service.GetByIdAsync(id, ct);
            if (existing == null)
            {
                return NotFound(new { Message = $"Entity with ID {id} not found." });
            }

            await _service.DeleteAsync(id, ct);
            return NoContent();
        }


    }
}
