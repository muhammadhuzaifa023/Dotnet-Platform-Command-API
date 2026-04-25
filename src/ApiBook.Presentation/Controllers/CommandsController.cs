using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBook.Presentation.Controllers;

[ApiController]
[Route("api/commands")]
public class CommandsController(ICommandService commandService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var commands = await commandService.GetAllAsync(cancellationToken);
        return Ok(commands);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var command = await commandService.GetByIdAsync(id, cancellationToken);
        return command is null ? NotFound() : Ok(command);
    }

    [HttpGet("platform/{platformId:int}")]
    public async Task<IActionResult> GetByPlatformId(int platformId, CancellationToken cancellationToken)
    {
        var commands = await commandService.GetByPlatformIdAsync(platformId, cancellationToken);
        return Ok(commands);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommandCreateDto request, CancellationToken cancellationToken)
    {
        var created = await commandService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created!.Id }, created);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CommandUpdateDto request, CancellationToken cancellationToken)
    {
        var updated = await commandService.UpdateAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await commandService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
