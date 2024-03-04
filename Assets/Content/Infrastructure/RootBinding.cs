using Content.Infrastructure.AssetManagement;
using Content.Infrastructure.Factories;
using Content.Infrastructure.Factories.Interfaces;
using Content.Infrastructure.SceneManagement;
using Content.Infrastructure.Services.Logging;
using Content.Infrastructure.Services.PersistentData;
using Content.Infrastructure.Services.SaveLoad;
using Content.Infrastructure.States;
using Content.Infrastructure.States.Interfaces;
using UnityEngine;

namespace Content.Infrastructure
{
    public class RootBinding : MonoBehaviour
    {
        private void Awake()
        {
            RegisterProviders();
            CreateAndRegisterFactories();
            RegisterServices();
            RegisterStates();
        }

        private void RegisterProviders()
        {
            DIContainer.Container.RegisterService<IAssetProvider>(new AssetProvider());
            DIContainer.Container.RegisterService<ISceneLoader>(new SceneLoader());
        }

        private void CreateAndRegisterFactories()
        {
            DIContainer.Container.RegisterService<IUIFactory>(
                new UIFactory(DIContainer.Container.GetService<IAssetProvider>()));
        }

        private void RegisterServices()
        {
            DIContainer.Container.RegisterService<ILoggingService>(new LoggingService());
            DIContainer.Container.RegisterService<IPersistentDataService>(new PersistentDataService());
            DIContainer.Container.RegisterService<ISaveLoadService>(
                new SaveLoadService(DIContainer.Container.GetService<IPersistentDataService>(),
                    DIContainer.Container.GetService<IAssetProvider>(),
                    DIContainer.Container.GetService<ILoggingService>()));
        }

        private void RegisterStates()
        {
            GameStateMachine gameStateMachine = new GameStateMachine(new StateFactory(),
                DIContainer.Container.GetService<ILoggingService>());

            DIContainer.Container.RegisterService<IStateMachine>(gameStateMachine);
            gameStateMachine.Initialize();
        }
    }
}