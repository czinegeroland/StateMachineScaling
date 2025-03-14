namespace Consumer.OrderStateMachine.ShippingOrder
{
    public record OrderShipped : IOrderId
    {
        public Guid OrderId { get; set; }
    }
}
