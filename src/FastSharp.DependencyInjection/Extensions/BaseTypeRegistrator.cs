using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using FastSharp.DependencyInjection.Attributes;

namespace FastSharp.DependencyInjection.Extensions
{
    /// <summary>
    /// Base Type Registrator Extensions
    /// </summary>
    public static class BaseTypeRegistrator
    {
        /// <summary>
        /// Register services base in base type.
        /// If <see cref="InjectAttribute"/> is not set the class will be registered as <see cref="ServiceLifetime.Scoped"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterByBaseType(this IServiceCollection services, Assembly assembly, Type baseType)
        {
            var types = assembly!.GetTypes()
                .Where(e =>
                    e.IsClass &&
                    !e.IsAbstract &&
                    baseType.IsAssignableFrom(e));

            foreach (var type in types)
                Register(services, type);

            return services;
        }

        private static void Register(IServiceCollection services, Type implementation)
        {
            var attribute = implementation.GetCustomAttribute<InjectAttribute>();
            if (attribute is null)
            {
                services.AddScoped(implementation);
                return;
            }

            services.Add(new ServiceDescriptor(implementation, attribute.LifeTime));
        }
    }
}
