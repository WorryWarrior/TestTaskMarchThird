namespace Content.Infrastructure.States.Interfaces
{
    public interface IState : IExitableState
    {
        public void Enter();
    }
}