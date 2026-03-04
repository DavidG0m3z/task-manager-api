using FluentValidation;

namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("El titulo es obligatorio")
            .MaximumLength(200)
            .WithMessage("El titulo no puede exceder 200 caractares");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("La descripcion no puede exceder 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Debe especificar una categoria valida");

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 3)
            .WithMessage("La prioridad debe ser (1): baja, (2): media, (3): alta");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("La fecha límite debe ser en el futuro")
            .When(x => x.DueDate.HasValue);

    }

}