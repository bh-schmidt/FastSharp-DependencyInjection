using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace TextsToStudy.Infra.IoC
{
    /// <summary>
    /// Attribute Registrator Extensions
    /// </summary>
    public static class AttributeRegistrator
    {
        /// <summary>
        /// Register services based on <see cref="InjectAttribute"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterByAttributes(this IServiceCollection services, Assembly assembly)
        {
            var implementations = assembly.GetTypes()
                .Where(a => a.IsClass && !a.IsAbstract);

            foreach (var implementation in implementations)
            {
                if (services.Any(s => s.ServiceType == implementation))
                    continue;

                var attribute = implementation.GetCustomAttribute<InjectAttribute>();
                if (attribute is null)
                    continue;

                services.Add(new ServiceDescriptor(implementation, attribute.LifeTime));
            }

            return services;
        }
    }
}
