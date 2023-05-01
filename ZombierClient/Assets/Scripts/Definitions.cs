using System.IO;

namespace Prototype
{
    public static class Definitions
    {
        private const string _RELATIVE_PATH_CONFIG_META = "../Configs/meta.json";
        private const string _RELATIVE_PATH_CONFIG_USER = "../Configs/user.json";
        private const string _RELATIVE_PATH_CONFIG_SESSION = "../Configs/session.json";
        public static string PATH_CONFIG_META => Path.Combine(Path.Combine(UnityEngine.Application.dataPath, _RELATIVE_PATH_CONFIG_META));
        public static string PATH_CONFIG_USER => Path.Combine(Path.Combine(UnityEngine.Application.dataPath, _RELATIVE_PATH_CONFIG_USER));
        public static string PATH_CONFIG_SESSION => Path.Combine(Path.Combine(UnityEngine.Application.dataPath, _RELATIVE_PATH_CONFIG_SESSION));

        public static float DESTROY_PROJECTILE_AFTER_TIME_SEC = 5f;
        //
        public const string TAG_VIEW_PFAB_UI_ENEMY = "ui.enemy.healthBar";
        public const string TAG_VIEW_PFAB_UI_TEXT_HIT = "text.hit";
    }
}
