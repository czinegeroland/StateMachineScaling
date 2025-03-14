namespace Consumer.OrderStateMachine.ProcessingPayment
{
    public record ProcessPayment : IOrderId
    {
        public Guid OrderId { get; set; }
    }
}
