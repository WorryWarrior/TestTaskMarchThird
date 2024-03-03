using System.Threading.Tasks;
using Content.Data;

namespace Content.Infrastructure.Services.SaveLoad
{
    public interface ISaveLoadService : IService
    {
        void SaveUserFavorites();
        Task<UserFavoriteData> LoadUserFavourite();
        Task<UserConfigData> LoadUserConfig();
        void SaveUserPictures();
        Task<UserPictureData> LoadUserPictures();
    }
}