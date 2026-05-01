using ApiBook.Application.Common;
using ApiBook.Application.Contracts;
using ApiBook.Application.DTOs;
using FluentValidation;
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

        return Ok(ApiResponse<object>.SuccessResponse(platforms, Messages.Fetched));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var platform = await platformService.GetByIdAsync(id, cancellationToken);

        if (platform is null)
            return NotFound(ApiResponse<string>.Fail(Messages.NotFound));

        return Ok(ApiResponse<object>.SuccessResponse(platform, Messages.Fetched));
    }

    // 🔥 CREATE
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] PlatformCreateDto request,
        [FromServices] IValidator<PlatformCreateDto> validator,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed"));

        var created = await platformService.CreateAsync(request, cancellationToken);

        return Ok(ApiResponse<object>.SuccessResponse(created, Messages.Created));
    }

    // 🔥 UPDATE
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] PlatformUpdateDto request,
        [FromServices] IValidator<PlatformUpdateDto> validator,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);

        if (!result.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Validation failed"));

        var updated = await platformService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
            return NotFound(ApiResponse<string>.Fail(Messages.NotFound));

        return Ok(ApiResponse<string>.SuccessResponse(null!, Messages.Updated));
    }

    // 🔥 DELETE
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await platformService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound(ApiResponse<string>.Fail(Messages.NotFound));

        return Ok(ApiResponse<string>.SuccessResponse(null!, Messages.Deleted));
    }
}