using UnityEditor;
using UnityEditor.SceneManagement;

namespace BiReJeJoCo.Tools
{
    public class SceneQuickAccess : EditorWindow
    {
        [MenuItem("Tools/Scenes/Main")]
        public static void OpenMainScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/main_scene.unity");
        }

        [MenuItem("Tools/Scenes/Login")]
        public static void OpenLoginScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/login_scene.unity");
        }

        [MenuItem("Tools/Scenes/Testing")]
        public static void OpenTestingScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/testing_scene.unity");
        }
    }
}