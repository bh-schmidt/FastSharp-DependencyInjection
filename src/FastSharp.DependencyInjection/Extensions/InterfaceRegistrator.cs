using FastSharp.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastSharp.DependencyInjection.Attributes;

namespace FastSharp.DependencyInjection.Extensions
{
    /// <summary>
    /// Interface Registrator Extensions.
    /// </summary>
    public static class InterfaceRegistrator
    {
        /// <summary>
        /// Register the interfaces of the assembly as services. 
        /// The interface name must follow the pattern I{ClassName}. 
        /// If <see cref="InjectAttribute"/> is not set the class will be registered as <see cref="ServiceLifetime.Scoped"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <exception cref="ApplicationException">When there is two classes implementing the same interface.</exception>
        /// <returns></returns>
        public static IServiceCollection RegisterInterfaces(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
                services.RegisterInterfacesInternal(assembly, DuplicationOptions.ThrowOnDuplicate);

            return services;
        }

        /// <summary>
        /// Register the interfaces of the assembly as services. 
        /// The interface name must follow the pattern I{ClassName}.
        /// If <see cref="InjectAttribute"/> is not set the class will be registered as <see cref="ServiceLifetime.Scoped"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="registratorOptions"></param>
        /// <param name="assemblies"></param>
        /// <exception cref="ApplicationException">When <paramref name="registratorOptions"/> is set to throw and there is two classes implementing the same interface.</exception>
        /// <returns></returns>
        public static IServiceCollection RegisterInterfaces(this IServiceCollection services, DuplicationOptions registratorOptions, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
                services.RegisterInterfacesInternal(assembly, registratorOptions);

            return services;
        }

        private static IServiceCollection RegisterInterfacesInternal(this IServiceCollection services, Assembly assembly, DuplicationOptions registratorOptions)
        {
            var allTypes = assembly.GetTypes();

            var interfaces = allTypes
                .Where(e => e.IsInterface)
                .ToArray();

            var classes = allTypes
                .Where(a => a.IsClass && !a.IsAbstract)
                .ToArray();

            foreach (var contract in interfaces)
            {
                if (services.Any(s => s.ServiceType == contract))
                    continue;

                var types = classes.Where(type =>
                   contract.IsAssignableFrom(type) &&
                   $"I{type.Name}" == contract.Name);

                if (!types.Any())
                    continue;


                if (types.Count() > 1 && registratorOptions == DuplicationOptions.Ignore)
                        continue;

                var implementation = GetImplementations(registratorOptions, contract, types);

                Register(services, contract, implementation);
            }

            return services;
        }

        private static Type GetImplementations(DuplicationOptions registratorOptions, Type contract, IEnumerable<Type> types)
        {
            if (types.Count() > 1 && registratorOptions == DuplicationOptions.ThrowOnDuplicate)
                throw new ApplicationException($"There are {types.Count()} implementations for the interface {contract.FullName}.\nImplementations:\n\t{string.Join("\n\t", types.Select(e => e.FullName))}");

            return types.First();
        }

        private static void Register(IServiceCollection services, Type service, Type implementation)
        {
            var attribute = implementation.GetCustomAttribute<InjectAttribute>() ??
                service.GetCustomAttribute<InjectAttribute>();

            if (attribute is null)
            {
                services.AddScoped(service, implementation);
                return;
            }

            services.Add(new ServiceDescriptor(service, implementation, attribute.LifeTime));
        }
    }
}
