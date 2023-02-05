using Avangardum.TwilightRun.Main;
using UnityEditor;

namespace Avangardum.TwilightRun.Tests
{
    public static class TestUtil
    {
        public static GameConfig GameConfig => 
            AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/Twilight Run/Game Config.asset");
    }
}