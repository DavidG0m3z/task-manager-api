using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.Features.Auth.Commands.Registrer
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El email no es válido")
                .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
                .Matches(@"[A-Z]").WithMessage("La contraseña debe tener al menos una mayúscula")
                .Matches(@"[a-z]").WithMessage("La contraseña debe tener al menos una minúscula")
                .Matches(@"\d").WithMessage("La contraseña debe tener al menos un número")
                .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage("La contraseña debe tener al menos un carácter especial");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es obligatorio")
                .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres");
        }
    }
}
