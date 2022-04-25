using Microsoft.Extensions.DependencyInjection;
using System;

namespace FastSharp.DependencyInjection.Attributes
{
    /// <summary>
    /// Attribute to define the class as injectable and set its lifetime.
    /// </summary>
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// Configures the attribute to <see cref="ServiceLifetime.Scoped"/>.
        /// </summary>
        public InjectAttribute()
        {
            LifeTime = ServiceLifetime.Scoped;
        }

        /// <summary>
        /// Configures the attribute to <paramref name="lifeTime"/>
        /// </summary>
        public InjectAttribute(ServiceLifetime lifeTime = ServiceLifetime.Scoped)
        {
            LifeTime = lifeTime;
        }

        /// <summary>
        /// Represents the life time of the service.
        /// </summary>
        public ServiceLifetime LifeTime { get; }
    }
}
