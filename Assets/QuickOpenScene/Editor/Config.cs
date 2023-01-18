namespace QuickOpenScene
{
    public class Config
    {
        //版本
        public const string version = "1.5";        

        //菜单路径
        public struct MenuPath
        {
            public const string quickOpenSceneWindow = "Tools/Quick Open Scene/快速打开场景 %&X";
            public const string aboutWindow = "Tools/Quick Open Scene/关于";
            public const string addCurrScene = "Assets/Tools/Quick Open Scene/添加当前目录场景或此场景到配置文件";
            public const string addAllScene = "Assets/Tools/Quick Open Scene/添加所有场景到配置文件";
        }

        public struct About
        {
            public const string githubURL = "https://github.com/BoyceLig/QuickOpenScene"; 
            public const string githubIssuesURL = "https://github.com/BoyceLig/QuickOpenScene/issues"; 
            public const string githubReleasesURL = "https://github.com/BoyceLig/QuickOpenScene/releases"; 
            public const string githubLatestAPI = "https://api.github.com/repos/BoyceLig/QuickOpenScene/releases/latest"; 
        }
    }
}