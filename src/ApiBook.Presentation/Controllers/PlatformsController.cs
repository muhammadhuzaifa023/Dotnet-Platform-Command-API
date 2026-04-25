using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBook.Presentation.Controllers;

[ApiController]
[Route("api/platforms")]
public class PlatformsController(IPlatformService platformService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var platforms = await platformService.GetAllAsync(cancellationToken);
        return Ok(platforms);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var platform = await platformService.GetByIdAsync(id, cancellationToken);
        return platform is null ? NotFound() : Ok(platform);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] PlatformCreateDto request,
        CancellationToken cancellationToken)
    {
        var created = await platformService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] PlatformUpdateDto request,
        CancellationToken cancellationToken)
    {
        var updated = await platformService.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await platformService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
