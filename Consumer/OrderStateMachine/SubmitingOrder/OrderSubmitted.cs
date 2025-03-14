namespace Consumer.OrderStateMachine.SubmitingOrder
{
    public record OrderSubmitted : IOrderId
    {
        public Guid OrderId { get; set; }
    }
}
