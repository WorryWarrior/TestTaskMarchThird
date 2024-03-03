using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Content.Infrastructure.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        public async Task<SceneInstance> LoadScene(SceneName sceneName, Action<SceneName> onLoaded = null)
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName.ToSceneString());
            await handle.Task;
            SceneInstance scene = handle.Result;
            scene.ActivateAsync();
            
            onLoaded?.Invoke(sceneName);
            return scene;
        }
    }
}