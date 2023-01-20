using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    [InitializeOnLoad]
    public class InvalidDataChecker : Config
    {
        static InvalidDataChecker()
        {
            new GetVersionInformation().GetJson();
            if (NeedUpdate && !EditorWindow.HasOpenInstances<AboutWindow>() && AutoOpenAbout == 0)
            {
                AboutWindow.OpenAbout();
            }
        }
    }
}
