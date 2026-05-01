using ApiBook.Application.DTOs;
using FluentValidation;

public class PlatformCreateValidator : AbstractValidator<PlatformCreateDto>
{
    public PlatformCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Publisher)
            .NotEmpty()
            .MaximumLength(100);
    }
}