using MassTransit;

namespace Consumer.OrderStateMachine.ShippingOrder
{
    public class ShipOrderConsumer : IConsumer<ShipOrder>
    {
        public async Task Consume(ConsumeContext<ShipOrder> context)
        {
            await context.Publish<OrderShipped>(new
            {
                context.Message.OrderId
            },
            context.CancellationToken);
        }
    }
}
