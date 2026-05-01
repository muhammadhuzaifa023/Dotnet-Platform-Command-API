using ApiBook.Application.DTOs;
using FluentValidation;

public class PlatformUpdateValidator : AbstractValidator<PlatformUpdateDto>
{
    public PlatformUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Publisher)
            .NotEmpty()
            .MaximumLength(100);
    }
}