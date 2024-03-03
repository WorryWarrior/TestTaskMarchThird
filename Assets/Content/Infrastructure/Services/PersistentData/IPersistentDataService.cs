using Content.Data;

namespace Content.Infrastructure.Services.PersistentData
{
    public interface IPersistentDataService : IService
    {
        UserConfigData UserConfig { get; set; }
        UserFavoriteData UserFavorite { get; set; }
        UserPictureData UserPictures { get; set; }
    }
}