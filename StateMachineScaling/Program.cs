using System.Net.Sockets;
using System.Security.Authentication;
using Consumer.OrderStateMachine.SubmitingOrder;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(busConfigurator =>
{
    ConnectionFactory.DefaultAddressFamily = AddressFamily.InterNetwork;

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/submitOrder", async ([FromServices] IPublishEndpoint publishEndpoint) =>
{
    await publishEndpoint.Publish(new OrderSubmitted
    {
        OrderId = NewId.NextGuid(),
    });

    return;
});

app.MapGet("/submitOrders-parallel/{messageCount:int}", async ([FromRoute] int messageCount,[FromServices] IPublishEndpoint publishEndpoint) =>
{
    var messages = Enumerable.Range(0, messageCount).Select(_ => publishEndpoint.Publish(new OrderSubmitted
    {
        OrderId = NewId.NextGuid(),
    }));

    await Task.WhenAll(messages);

    return;
});

app.Run();
