using Content.Data;

namespace Content.Infrastructure.Services.PersistentData
{
    public class PersistentDataService : IPersistentDataService
    {
        public UserConfigData UserConfig { get; set; }
        public UserFavoriteData UserFavorite { get; set; }
        public UserPictureData UserPictures { get; set; }
    }
}