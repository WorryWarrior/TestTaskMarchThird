using Content.Infrastructure.AssetManagement;
using Content.Infrastructure.Factories.Interfaces;
using Content.Infrastructure.SceneManagement;
using Content.Infrastructure.Services.Logging;
using Content.Infrastructure.Services.PersistentData;
using Content.Infrastructure.Services.SaveLoad;
using Content.Infrastructure.States;
using Content.Infrastructure.States.Interfaces;

namespace Content.Infrastructure.Factories
{
    public class StateFactory
    {
        public IExitableState CreateState<T>() where T : IExitableState
        {
            if (typeof(T) == typeof(BootstrapState))
            {
                return new BootstrapState(DIContainer.Container.GetService<IStateMachine>());
            }

            if (typeof(T) == typeof(LoadProgressState))
            {
                return new LoadProgressState(DIContainer.Container.GetService<IStateMachine>(),
                    DIContainer.Container.GetService<ISaveLoadService>(),
                    DIContainer.Container.GetService<IPersistentDataService>(),
                    DIContainer.Container.GetService<IAssetProvider>(),
                    DIContainer.Container.GetService<ILoggingService>());
            }

            if (typeof(T) == typeof(LoadGameState))
            {
                return new LoadGameState(DIContainer.Container.GetService<IStateMachine>(),
                    DIContainer.Container.GetService<IUIFactory>(),
                    DIContainer.Container.GetService<ISceneLoader>(),
                    DIContainer.Container.GetService<IPersistentDataService>(),
                    DIContainer.Container.GetService<ISaveLoadService>());
            }

            return null;
        }
    }
}