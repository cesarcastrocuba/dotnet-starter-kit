using FluentValidation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace FSH.Framework.Web.Modules;

public static class ModuleLoader
{
    private sealed class ModuleRegistry
    {
        public List<IModule> Modules { get; } = new();
    }

    public static IHostApplicationBuilder AddModules(this IHostApplicationBuilder builder, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // Check if modules were already added to this builder's services
        if (builder.Services.Any(d => d.ServiceType == typeof(ModuleRegistry)))
        {
            return builder;
        }

        var registry = new ModuleRegistry();
        builder.Services.AddSingleton(registry);

        builder.Services.AddValidatorsFromAssemblies(assemblies);

        var source = assemblies is { Length: > 0 }
            ? assemblies
            : AppDomain.CurrentDomain.GetAssemblies();

        var moduleRegistrations = source
            .SelectMany(a => a.GetCustomAttributes<FshModuleAttribute>())
            .Where(r => typeof(IModule).IsAssignableFrom(r.ModuleType))
            .DistinctBy(r => r.ModuleType)
            .OrderBy(r => r.Order)
            .ThenBy(r => r.ModuleType.Name)
            .Select(r => r.ModuleType)
            .ToList();

        // Fallback: if no modules found via attribute, scan for IModule implementations directly
        if (moduleRegistrations.Count == 0 && assemblies is { Length: > 0 })
        {
            moduleRegistrations = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();
        }

        foreach (var moduleType in moduleRegistrations)
        {
            if (Activator.CreateInstance(moduleType) is not IModule module)
            {
                throw new InvalidOperationException($"Unable to create module {moduleType.Name}.");
            }

            module.ConfigureServices(builder);
            registry.Modules.Add(module);
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var registry = endpoints.ServiceProvider.GetRequiredService<ModuleRegistry>();
        foreach (var m in registry.Modules)
            m.MapEndpoints(endpoints);

        return endpoints;
    }
}
