using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TaskManager.Application;

public static class DependencyInyection
{
    public static IServiceCollection AddApplication(this IServiceCollection service)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        service.addValidatorsFromAssembly(Assembly.GetExecutingAssemby());

        return service;

    }
}