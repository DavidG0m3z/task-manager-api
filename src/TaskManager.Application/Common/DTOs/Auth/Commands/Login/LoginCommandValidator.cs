using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.Common.DTOs.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator() 
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email obligatorio")
                .EmailAddress()
                .WithMessage("Email No valido");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Pass Obligatorio");
        }
    }
}
