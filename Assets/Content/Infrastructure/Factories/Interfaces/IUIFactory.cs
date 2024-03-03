using System.Threading.Tasks;
using Content.UI.MainScreen;

namespace Content.Infrastructure.Factories.Interfaces
{
    public interface IUIFactory : IService
    {
        Task WarmUp();
        void CleanUp();
        Task CreateUIRoot();
        Task<HudController> CreateHud();
        Task<UserInfoBoxController> CreateUserInfoBox(HudController hudController, bool asFavorite = false);
    }
}