using Content.Infrastructure.SceneManagement;
using Content.Infrastructure.States.Interfaces;

namespace Content.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly ISceneLoader _sceneLoader;

        public BootstrapState(
            IStateMachine gameStateMachine)
        {
            _stateMachine = gameStateMachine;
        }

        public void Enter()
        {
            _stateMachine.Enter<LoadProgressState>();
        }

        public void Exit()
        {

        }
    }
}