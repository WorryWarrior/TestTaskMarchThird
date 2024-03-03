using System;

namespace Content.Infrastructure.SceneManagement
{
    public static class SceneNameExtensions
    {
        public static SceneName ToSceneName(this string sceneName)
        {
            return sceneName switch
            {
                "SCN_Boot" => SceneName.Boot,
                "SCN_Core" => SceneName.Core,
                _          => throw new ArgumentOutOfRangeException(nameof(sceneName), sceneName, null)
            };
        }

        public static string ToSceneString(this SceneName sceneName)
        {
            return sceneName switch
            {
                SceneName.Boot => "SCN_Boot",
                SceneName.Core => "SCN_Core",
                _              => throw new ArgumentOutOfRangeException(nameof(sceneName), sceneName, null)
            };
        }
    }
}