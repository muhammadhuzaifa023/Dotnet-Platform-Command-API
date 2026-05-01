using ApiBook.Application.Common;
using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using FluentValidation;
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

        return Ok(ApiResponse<object>.SuccessResponse(commands, "Records fetched successfully"));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var command = await commandService.GetByIdAsync(id, cancellationToken);

        if (command is null)
            return NotFound(ApiResponse<string>.Fail("Record not found"));

        return Ok(ApiResponse<object>.SuccessResponse(command, "Record fetched successfully"));
    }

    [HttpGet("platform/{platformId:int}")]
    public async Task<IActionResult> GetByPlatformId(int platformId, CancellationToken cancellationToken)
    {
        var commands = await commandService.GetByPlatformIdAsync(platformId, cancellationToken);

        return Ok(ApiResponse<object>.SuccessResponse(commands, "Records fetched successfully"));
    }

    // 🔥 CREATE
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CommandCreateDto request,
        [FromServices] IValidator<CommandCreateDto> validator,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed"));

        var created = await commandService.CreateAsync(request, cancellationToken);

        return Ok(ApiResponse<object>.SuccessResponse(created, "Record created successfully"));
    }

    // 🔥 UPDATE
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] CommandUpdateDto request,
        [FromServices] IValidator<CommandUpdateDto> validator,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed"));

        var updated = await commandService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
            return NotFound(ApiResponse<string>.Fail("Record not found"));

        return Ok(ApiResponse<string>.SuccessResponse(null!, "Record updated successfully"));
    }

    // 🔥 DELETE
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await commandService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound(ApiResponse<string>.Fail("Record not found"));

        return Ok(ApiResponse<string>.SuccessResponse(null!, "Record deleted successfully"));
    }
}