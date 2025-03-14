using Consumer.OrderStateMachine.ProcessingPayment;
using Consumer.OrderStateMachine.ShippingOrder;
using Consumer.OrderStateMachine.SubmitingOrder;
using MassTransit;

namespace Consumer.StateMachine
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public State PaymentProcessing { get; private set; }
        public State PaymentProcessingFailed { get; private set; }

        public State Shipping { get; private set; }
        public State ShippingFailed { get; private set; }

        public Event<OrderSubmitted> OrderSubmitted { get; private set; }

        public Event<PaymentProcessed> PaymentProcessed { get; private set; }
        public Event<Fault<ProcessPayment>> PaymentProcessedFailed { get; private set; }

        public Event<OrderShipped> OrderShipped { get; private set; }
        public Event<Fault<ShipOrder>> OrderShippedFailed { get; private set; }

        public OrderStateMachine()
        {
            InstanceState(_ => _.CurrentState);

            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));

            Event(() => PaymentProcessed, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentProcessedFailed, x => x.CorrelateById(m => m.Message.Message.OrderId));

            Event(() => OrderShipped, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderShippedFailed, x => x.CorrelateById(m => m.Message.Message.OrderId));

            Initially(
                When(OrderSubmitted)
                    .Then(_ => _.Saga.OrderId = _.Message.OrderId)
                    .Publish(_ => new ProcessPayment 
                    { 
                        OrderId = _.Message.OrderId 
                    })
                    .TransitionTo(PaymentProcessing));

            During(PaymentProcessing,
                When(PaymentProcessed)
                    .Publish(_ => new ShipOrder
                    {
                        OrderId = _.Message.OrderId
                    })
                    .TransitionTo(Shipping),
                When(PaymentProcessedFailed)
                    .TransitionTo(PaymentProcessingFailed));

            During(Shipping,
                When(OrderShipped)
                    .Finalize(),
                 When(OrderShippedFailed)
                    .TransitionTo(ShippingFailed));                
        }
    }
}
