using NSubstitute;
using Shouldly;
using Wolverine.RabbitMQ.Internal;
using Wolverine.Runtime;
using Xunit;

namespace Wolverine.RabbitMQ.Tests;

public class RabbitMqListenerConfigurationTests
{
    [Fact]
    public void override_prefetch_count()
    {
        var endpoint = new RabbitMqQueue("foo", new RabbitMqTransport());
        var expression = new RabbitMqListenerConfiguration(endpoint, new RabbitMqTransport());

        expression.PreFetchCount(99).ShouldBeSameAs(expression);

        var wolverineRuntime = Substitute.For<IWolverineRuntime>();
        wolverineRuntime.Options.Returns(new WolverineOptions());

        endpoint.Compile(wolverineRuntime);

        endpoint.PreFetchCount.ShouldBe((ushort)99);
    }

    [Fact]
    public void override_queue_type()
    {
        var endpoint = new RabbitMqQueue("foo", new RabbitMqTransport());
        var expression = new RabbitMqListenerConfiguration(endpoint, new RabbitMqTransport());

        expression.QueueType(QueueType.quorum);
        
        var wolverineRuntime = Substitute.For<IWolverineRuntime>();
        wolverineRuntime.Options.Returns(new WolverineOptions());

        endpoint.Compile(wolverineRuntime);
        
        endpoint.QueueType.ShouldBe(QueueType.quorum);
    }

    [Fact]
    public void use_specialized_mapper()
    {
        var endpoint = new RabbitMqQueue("foo", new RabbitMqTransport());
        var expression = new RabbitMqListenerConfiguration(endpoint, new RabbitMqTransport());

        var theMapper = new SpecialMapper();
        expression.UseInterop(theMapper);

        var wolverineRuntime = Substitute.For<IWolverineRuntime>();
        wolverineRuntime.Options.Returns(new WolverineOptions());

        endpoint.Compile(wolverineRuntime);

        endpoint.BuildMapper(wolverineRuntime).ShouldBeSameAs(theMapper);
    }
}