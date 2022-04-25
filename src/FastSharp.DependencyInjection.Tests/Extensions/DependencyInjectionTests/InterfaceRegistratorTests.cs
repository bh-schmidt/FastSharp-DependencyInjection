using FastSharp.DependencyInjection.Extensions;
using FastSharp.DependencyInjection.Tests.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace FastSharp.DependencyInjection.Tests.Extensions.DependencyInjectionTests
{
    public class InterfaceRegistratorTests
    {
        private Mock<Assembly> assemblyMock = null!;

        [SetUp]
        public void Setup()
        {
            assemblyMock = new Mock<Assembly>();
        }

        [Test]
        public void Should_register_class_without_attribute()
        {
            var serviceCollection = new ServiceCollection();

            assemblyMock
                .Setup(e => e.GetTypes())
                .Returns(new[]
                {
                    typeof(ISimpleObject),
                    typeof(SimpleObject)
                });

            serviceCollection.RegisterInterfaces(assemblyMock.Object);
            serviceCollection.Count.Should().Be(1);
        }

        [Test]
        public void Should_register_class_with_attribute()
        {
            var serviceCollection = new ServiceCollection();

            assemblyMock
                .Setup(e => e.GetTypes())
                .Returns(new[]
                {
                    typeof(IAttributeObject),
                    typeof(AttributeObject)
                });

            serviceCollection.RegisterInterfaces(assemblyMock.Object);
            serviceCollection.Count.Should().Be(1);
        }

        [Test]
        public void Should_resolve_duplicity()
        {
            var services = new ServiceCollection();

            assemblyMock
                .Setup(e => e.GetTypes())
                .Returns(new[]
                {
                    typeof(ISimpleObject),
                    typeof(SimpleObject),
                    typeof(SimpleObject)
                });

            services.RegisterInterfaces(DuplicationOptions.TakeFirst, assemblyMock.Object);
            services.Count.Should().Be(1);
        }

        [Test]
        public void Should_not_increase_count_after_second_run()
        {
            var services = new ServiceCollection();

            assemblyMock
                .Setup(e => e.GetTypes())
                .Returns(new[]
                {
                    typeof(ISimpleObject),
                    typeof(SimpleObject)
                });

            services.RegisterInterfaces(assemblyMock.Object);

            services.Count.Should().BeGreaterThan(0);
            var count = services.Count;

            services.RegisterInterfaces(assemblyMock.Object);

            services.Count.Should().Be(count);
        }

        [Test]
        public void Should_not_map_because_there_is_no_implementation()
        {
            var services = new ServiceCollection();

            assemblyMock
                .Setup(e => e.GetTypes())
                .Returns(new[]
                {
                    typeof(ISimpleObject),
                    typeof(SimpleObject2),
                });

            services.RegisterInterfaces(assemblyMock.Object);
            services.Count.Should().Be(0);
        }

        [Test]
        public void Should_not_map_because_there_is_no_duplicity_resolver()
        {
            var services = new ServiceCollection();

            assemblyMock
                .Setup(e => e.GetTypes())
                .Returns(new[]
                {
                    typeof(ISimpleObject),
                    typeof(SimpleObject),
                    typeof(SimpleObject)
                });

            services.RegisterInterfaces(DuplicationOptions.Ignore, assemblyMock.Object);
            services.Count.Should().Be(0);
        }

        [Test]
        public void Should_throw_because_there_is_no_duplicity_resolver()
        {
            var services = new ServiceCollection();

            assemblyMock
                .Setup(e => e.GetTypes())
                .Returns(new[]
                {
                    typeof(ISimpleObject),
                    typeof(SimpleObject),
                    typeof(SimpleObject)
                });

            Assert.Catch(() => services.RegisterInterfaces(assemblyMock.Object));
            services.Count.Should().Be(0);
        }
    }
}
