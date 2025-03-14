using MassTransit;

namespace Consumer.OrderStateMachine.ProcessingPayment
{
    public class ProcessPaymentConsumer : IConsumer<ProcessPayment>
    {
        public async Task Consume(ConsumeContext<ProcessPayment> context)
        {
            await context.Publish<PaymentProcessed>(new
            {
                context.Message.OrderId
            },
            context.CancellationToken);
        }
    }
}
