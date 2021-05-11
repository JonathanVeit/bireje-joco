using UnityEditor;
using BiReJeJoCo.Backend;

namespace BiReJeJoCo.Tools
{
    public class SycTriggerSetup : EditorWindow
    {
        [MenuItem("Tools/SychronizedTrigger/Auto Setup")]
        public static void OpenLoginScene()
        {
            var allTrigger = FindObjectsOfType<SynchronizedTrigger>();

            for (int i = 0; i < allTrigger.Length; i++)
            {
                allTrigger[i].SetTriggerId((byte)i);
            }
        }
    }
}