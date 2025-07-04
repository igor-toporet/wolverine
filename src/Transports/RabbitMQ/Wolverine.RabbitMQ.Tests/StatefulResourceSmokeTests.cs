using JasperFx.Core;
using Microsoft.Extensions.Hosting;
using JasperFx;
using Shouldly;
using Xunit;

namespace Wolverine.RabbitMQ.Tests;

public class StatefulResourceSmokeTests
{
    private IHostBuilder ConfigureBuilder(bool autoProvision, int starting = 1)
    {
        return Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                if (autoProvision)
                {
                    opts.UseRabbitMq().AutoProvision();
                }
                else
                {
                    opts.UseRabbitMq();
                }

                opts.PublishMessage<SRMessage1>()
                    .ToRabbitQueue("sr" + starting++);

                opts.PublishMessage<SRMessage2>()
                    .ToRabbitQueue("sr" + starting++);

                opts.PublishMessage<SRMessage3>()
                    .ToRabbitQueue("sr" + starting++);

                opts.PublishMessage<SRMessage3>()
                    .ToRabbitQueue("sr" + starting++);

                PublishMessageToRabbitExchange<SRMessage4>(opts, "sr" + starting++);
            });

        void PublishMessageToRabbitExchange<TMessage>(WolverineOptions opts, string queueName)
            => opts.PublishMessage<TMessage>()
                .ToRabbitExchange(queueName, exchange => exchange.BindQueue(queueName));
    }

    [Fact]
    public async Task run_setup()
    {
        var result = await ConfigureBuilder(false)
            .RunJasperFxCommands(["resources", "setup"]);
        result.ShouldBe(0);
    }

    [Fact]
    public async Task statistics()
    {
        (await ConfigureBuilder(false)
            .RunJasperFxCommands(["resources", "setup"])).ShouldBe(0);

        var result = await ConfigureBuilder(false)
            .RunJasperFxCommands(["resources", "statistics"]);

        result.ShouldBe(0);
    }

    [Fact]
    public async Task check_positive()
    {
        (await ConfigureBuilder(false)
            .RunJasperFxCommands(["resources", "setup"])).ShouldBe(0);

        var result = await ConfigureBuilder(false)
            .RunJasperFxCommands(["resources", "check"]);

        result.ShouldBe(0);
    }

    [Fact]
    public async Task check_negative()
    {
        var result = await ConfigureBuilder(false, 10)
            .RunJasperFxCommands(["resources", "check"]);

        result.ShouldBe(1);
    }

    [Fact]
    public async Task clear_state()
    {
        (await ConfigureBuilder(false, 20)
            .RunJasperFxCommands(["resources", "setup"])).ShouldBe(0);

        (await ConfigureBuilder(false, 20)
            .RunJasperFxCommands(["resources", "clear"])).ShouldBe(0);
    }

    [Fact]
    public async Task teardown()
    {
        (await ConfigureBuilder(false, 30)
            .RunJasperFxCommands(["resources", "setup"])).ShouldBe(0);

        (await ConfigureBuilder(false, 30)
            .RunJasperFxCommands(["resources", "teardown"])).ShouldBe(0);
    }
}

public class SRMessage1;

public class SRMessage2;

public class SRMessage3;

public class SRMessage4;

public class SRMessageHandlers
{
    public Task Handle(SRMessage1 message)
    {
        return Task.Delay(100.Milliseconds());
    }

    public Task Handle(SRMessage2 message)
    {
        return Task.Delay(100.Milliseconds());
    }

    public Task Handle(SRMessage3 message)
    {
        return Task.Delay(100.Milliseconds());
    }

    public Task Handle(SRMessage4 message)
    {
        return Task.Delay(100.Milliseconds());
    }
}