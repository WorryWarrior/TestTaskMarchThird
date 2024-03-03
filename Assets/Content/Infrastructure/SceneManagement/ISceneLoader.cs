using System;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Content.Infrastructure.SceneManagement
{
    public interface ISceneLoader : IService
    {
        public Task<SceneInstance> LoadScene(SceneName sceneName, Action<SceneName> onLoaded = null);
    }
}