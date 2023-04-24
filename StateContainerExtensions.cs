using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Inferno;
public static class StateContainerExtensions
{
    public static IServiceCollection RegisterStateContainers(
        this IServiceCollection services,
        Assembly assembly,
        string? folderName = "StateContainers")
    {
        var containerType = typeof(StateContainerBase);

        var entryNamespace = assembly.GetName().Name;
        var fullContainerNamespace = $"{entryNamespace}.{folderName}";

        var containerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(containerType) && t.Namespace == fullContainerNamespace)
            .ToList();

        var isWasm = OperatingSystem.IsBrowser();

        foreach (var container in containerTypes)
        {
            if (isWasm)
            {
                services.AddSingleton(container);
            }
            else
            {
                services.AddScoped(container);
            }
        }

        return services;
    }
} 