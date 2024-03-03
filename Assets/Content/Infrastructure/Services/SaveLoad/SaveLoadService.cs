using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Content.Data;
using Content.Infrastructure.AssetManagement;
using Content.Infrastructure.Services.PersistentData;
using static Newtonsoft.Json.JsonConvert;
using UnityEngine;

namespace Content.Infrastructure.Services.SaveLoad
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string UserFavoriteKey = "CFG_UserFavorite";
        private const string UserConfigKey = "CFG_UserData";
        private const string UserPicturesSaveFileName = "UserPictures.bin";

        private readonly IPersistentDataService _persistentDataService;
        private readonly IAssetProvider _assetProvider;

        public SaveLoadService(
            IPersistentDataService persistentDataService,
            IAssetProvider assetProvider)
        {
            _persistentDataService = persistentDataService;
            _assetProvider = assetProvider;
        }

        public void SaveUserFavorites()
        {
            string userFavoriteJson = SerializeObject(_persistentDataService.UserFavorite);
            PlayerPrefs.SetString(UserFavoriteKey, userFavoriteJson);
        }

        public Task<UserFavoriteData> LoadUserFavourite()
        {
            UserFavoriteData userFavoriteData =
                DeserializeObject<UserFavoriteData>(PlayerPrefs.GetString(UserFavoriteKey));
            return Task.FromResult(userFavoriteData);
        }

        public async Task<UserConfigData> LoadUserConfig()
        {
            TextAsset userDataFile = await _assetProvider.Load<TextAsset>(UserConfigKey);
            string userDataJson = userDataFile.text;
            UserConfigData userConfigData = DeserializeObject<UserConfigData>(userDataJson);

            return userConfigData;
        }

        public void SaveUserPictures()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            using FileStream fileStream = File.Create(GetUserPicturesSaveFilePath());
            List<byte[]> spriteData = _persistentDataService.UserPictures.UserPictures
                .Select(userPicture => userPicture.texture.EncodeToPNG()).ToList();

            binaryFormatter.Serialize(fileStream, spriteData);
        }

        public Task<UserPictureData> LoadUserPictures()
        {
            if (!File.Exists(GetUserPicturesSaveFilePath()))
                return Task.FromResult((UserPictureData)null);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(GetUserPicturesSaveFilePath(), FileMode.Open);

            List<byte[]> spriteData = (List<byte[]>)binaryFormatter.Deserialize(fileStream);
            List<Sprite> sprites = new List<Sprite>();

            foreach (byte[] bytes in spriteData)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                sprites.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
            }

            UserPictureData userFavoriteData = new UserPictureData
            {
                UserPictures = sprites
            };

            return Task.FromResult(userFavoriteData);
        }

        private string GetUserPicturesSaveFilePath() =>
            $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}{UserPicturesSaveFileName}";
    }
}