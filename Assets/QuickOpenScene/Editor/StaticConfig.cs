namespace QuickOpenScene
{
    public class StaticConfig
    {
        //快捷键枚举
        public enum Key
        {
            None, Ctrl, Shift, Alt
        }

        public static Key key1;
        public static Key key2;
        public static Key key3;
        public static string key4;

        public static Key defaultKey1 = Key.None;
        public static Key defaultKey2 = Key.Ctrl;
        public static Key defaultKey3 = Key.Alt;
        public static string defaultKey4 = "X";


        //配置文件位置
        public static string sceneConfigInfoPath;
        public static string defaultSceneConfigInfoPath = "Assets/QuickOpenScene/Data/SceneConfig.asset";
        public const string VERSION = "1.1";

    }
}
