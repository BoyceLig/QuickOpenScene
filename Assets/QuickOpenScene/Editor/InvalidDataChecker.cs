using UnityEditor;

namespace QuickOpenScene
{
    [InitializeOnLoad]
    public class InvalidDataChecker : Config
    {
        static InvalidDataChecker()
        {
            new GetVersionInformation().GetJson();
            if (NeedUpdate)
            {
                AboutWindow.OpenAbout();
            }
        }
    }
}
