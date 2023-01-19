using UnityEditor;
using UnityEngine;

namespace QuickOpenScene
{
    [InitializeOnLoad]
    public class Startup : MonoBehaviour
    {
        static Startup()
        {
            if (!Config.IsDown)
            {
                new HttpUitls().GetJson();
                Config.IsDown = true;                
            }
        }
    }
}
