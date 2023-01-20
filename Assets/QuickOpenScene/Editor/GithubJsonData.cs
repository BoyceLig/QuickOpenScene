using System;
using System.Collections.Generic;

namespace QuickOpenScene
{
    [Serializable]
    public class GithubJsonData
    {
        public string tag_name;
        public assets[] assets;
    }

    [Serializable]
    public class assets
    {
        public string browser_download_url;
    }
}