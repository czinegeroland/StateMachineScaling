using Consumer.OrderStateMachine;
using MassTransit;

namespace Consumer.StateMachine
{
    public record OrderState : SagaStateMachineInstance, ISagaVersion, IOrderId
    {
        public Guid CorrelationId { get; set; }

        public Guid OrderId { get; set; }

        public string CurrentState { get; set; }

        public int Version { get; set; }
    }
}
