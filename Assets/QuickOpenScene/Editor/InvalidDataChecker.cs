using System;
using UnityEditor;

namespace QuickOpenScene
{
    [InitializeOnLoad]
    public class InvalidDataChecker : Config
    {
        static InvalidDataChecker()
        {
            if (UpdateTime.AddHours(24) <= DateTime.Now)
            {
                new GetVersionInformation().GetJson();
            }

            if (NeedUpdate && !EditorWindow.HasOpenInstances<AboutWindow>() && AutoOpenAbout == 0)
            {
                AboutWindow.OpenAbout();
            }
        }
    }
}
