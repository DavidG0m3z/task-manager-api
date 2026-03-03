using FluentValidation;

namespace TaskManager.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator ()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage($"Debe especificar un Id valido");

        RuleFor(x => x.Title)
            .NoEmpty()
            .WithMessage($"El titulo es obligatorio")
            .MaximunLength(200)
            .WithMessage($"El titulo no puede exceder 200 caractares");

        RuleFor(x => x.Description)
            .MaximumLength("La descripcion no puede exceder 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Priority)
            .InclusiveBetween(1,3)
            .WithMessage($"La prioridad debe ser 1 (Baja), 2 (Media) o 3 (Alta)"):

        RuleFor(x => x.DueDate)
            //.GreaterThan(DateTime.UtcNow)
            .WithMessage("La fecha limite debe ser en el futuro")
            .When(x => x.DueDate.HasValue);
    }
}