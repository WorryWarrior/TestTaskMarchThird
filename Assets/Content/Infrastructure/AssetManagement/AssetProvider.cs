using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Content.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        private readonly Dictionary<string, AsyncOperationHandle> _completedCache = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();
        
        public void Initialize() => Addressables.InitializeAsync();

        public async Task<T> Load<T>(string key) where T : class
        {
            if (_completedCache.TryGetValue(key, out var completedHandle))
                return completedHandle.Result as T;
      
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);

            return await RunWithCacheOnComplete(handle, key);
        }

        public void Release(string key)
        {
            if (!_handles.ContainsKey(key))
                return;
            
            foreach (AsyncOperationHandle handle in _handles[key])
                Addressables.Release(handle);

            _completedCache.Remove(key);
            _handles.Remove(key);
        }

        public void Cleanup()
        {
            if (_handles.Count == 0)
                return;
             
            foreach (List<AsyncOperationHandle> resourceHandles in _handles.Values)
                foreach (AsyncOperationHandle handle in resourceHandles)
                    Addressables.Release(handle);
      
            _completedCache.Clear();
            _handles.Clear();
        }
        
        private async Task<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey) where T : class
        {
            handle.Completed += completeHandle => _completedCache[cacheKey] = completeHandle;

            AddHandle(cacheKey, handle);
            return await handle.Task;
        }

        private void AddHandle(string key, AsyncOperationHandle handle)
        {
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
            {
                resourceHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourceHandles;
            }
            
            resourceHandles.Add(handle);
        }
    }
}