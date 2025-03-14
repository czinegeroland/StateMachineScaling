
namespace Consumer.OrderStateMachine.ProcessingPayment
{
    public record PaymentProcessed : IOrderId
    {
        public Guid OrderId { get; set; }
    }
}
