using System.Net.Sockets;
using System.Security.Authentication;
using Consumer.OrderStateMachine.ProcessingPayment;
using Consumer.OrderStateMachine.ShippingOrder;
using Consumer.StateMachine;
using MassTransit;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(busConfigurator =>
{
    ConnectionFactory.DefaultAddressFamily = AddressFamily.InterNetwork;

    busConfigurator
        .AddSagaStateMachine<OrderStateMachine, OrderState>()
        .Endpoint(_ => _.AddConfigureEndpointCallback((context, cfg) =>
        {
            cfg.UseMessageRetry(_ => _.Immediate(1));
            cfg.UseInMemoryInboxOutbox(context);
        }))
        .RedisRepository("merry-filly-*********.upstash.io:6379,password=********************************,ssl=True,abortConnect=False");

    busConfigurator
        .AddConsumer<ProcessPaymentConsumer>()
        .Endpoint(_ => _.AddConfigureEndpointCallback((context, cfg) =>
        {
            cfg.UseMessageRetry(_ => _.Immediate(2));
        }));
   
    busConfigurator
        .AddConsumer<ShipOrderConsumer>()
        .Endpoint(_ => _.AddConfigureEndpointCallback((context, cfg) =>
        {
            cfg.UseMessageRetry(_ => _.Immediate(2));
        }));

    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host("goose-01.rmq2.cloudamqp.com", 5671, "********", h =>
        {
            h.Username("*****************");
            h.Password("*****************");
            h.UseSsl(ssl =>
            {
                ssl.Protocol = SslProtocols.Tls12;
            });
        });

        configurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok());


app.Run();
