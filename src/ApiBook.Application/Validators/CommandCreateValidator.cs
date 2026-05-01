using FluentValidation;
using ApiBook.Application.DTOs;

namespace ApiBook.Application.Validators;

public class CommandCreateValidator : AbstractValidator<CommandCreateDto>
{
    public CommandCreateValidator()
    {
        RuleFor(x => x.HowTo)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CommandLine)
            .NotEmpty();

        RuleFor(x => x.PlatformId)
            .GreaterThan(0);
    }
}