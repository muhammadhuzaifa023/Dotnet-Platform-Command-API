using ApiBook.Application.DTOs;
using FluentValidation;

public class CommandUpdateValidator : AbstractValidator<CommandUpdateDto>
{
    public CommandUpdateValidator()
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