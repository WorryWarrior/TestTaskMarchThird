namespace Content.Infrastructure.States.Interfaces
{
    public interface IPayloadedState<TPayload> : IExitableState
    {
        public void Enter(TPayload payload);
    }
}