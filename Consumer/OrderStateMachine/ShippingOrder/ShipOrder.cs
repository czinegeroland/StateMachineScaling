namespace Consumer.OrderStateMachine.ShippingOrder
{
    public record ShipOrder : IOrderId
    {
        public Guid OrderId { get; set; }
    }
}
