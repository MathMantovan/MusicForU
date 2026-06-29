using Microsoft.AspNetCore.Mvc;
using MusicForU.Application.DTOs;
using MusicForU.Application.Interfaces;

namespace MusicForU.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanController : ControllerBase
{
    private readonly IRepository<Domain.Entities.Plan> _plans;
    public PlanController(IRepository<Domain.Entities.Plan> plans) => _plans = plans;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _plans.GetAllAsync();
        return Ok(plans.Select(p => new PlanDto(p.Id, p.Name, p.Price, p.DurationDays)));
    }
}
