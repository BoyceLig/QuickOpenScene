using UnityEditor;

namespace QuickOpenScene
{
    public class StaticConfig
    {
        //版本
        public const string version = "1.4";

        static string GetSceneConfigInfoPath()
        {
            string guid = AssetDatabase.FindAssets("QuickOpenSceneWindow")[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            path = path.Remove(path.IndexOf("QuickOpenScene/Editor")) + "QuickOpenScene/Data/SceneConfig.asset";
            return path;
        }
        public static SceneConfig GetSceneConfigObject()
        {
            SceneConfig sceneConfig = AssetDatabase.LoadAssetAtPath<SceneConfig>(GetSceneConfigInfoPath());
            return sceneConfig;
        }
    }
}
