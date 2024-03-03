using System.Threading.Tasks;

namespace Content.Infrastructure.AssetManagement
{
    public interface IAssetProvider : IService
    {
        public Task<T> Load<T>(string key) where T : class;
        public void Release(string key);
        public void Cleanup();
    }
}