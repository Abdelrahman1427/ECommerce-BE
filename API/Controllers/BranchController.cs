using API.Controllers;
using Application.Common.BranchDTOS;
using Application.Common.OrganizationDTOS;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Authorize(Roles = "SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class BranchController : GenericController<Branch, GetBranchDTO, object, AddBranchDTO, UpdateBranchDTO>
{
    private readonly IBranchService _service;
    public BranchController(IGenericService<Branch, GetBranchDTO, object, AddBranchDTO, UpdateBranchDTO> service, IBranchService branchService) : base(service)
    {
        _service = branchService;
    }
    [HttpGet("Lookup")]
    public async Task<ActionResult<List<OrganizationLookUp>>> Lookup()
    {
        var result = await _service.GetLookup();
        return Ok(result);
    }

}

