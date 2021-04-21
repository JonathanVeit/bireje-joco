using UnityEditor;
using UnityEditor.SceneManagement;

namespace BiReJeJoCo.Tools
{
    public class SceneQuickAccess : EditorWindow
    {
        [MenuItem("Tools/Scenes/Loading")]
        public static void OpenLoginScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/loading_scene.unity");
        }

        [MenuItem("Tools/Scenes/Main Menu")]
        public static void OpenMainScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/main_menu_scene.unity");
        }

        [MenuItem("Tools/Scenes/Lobby")]
        public static void OpenRoomMenu()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/lobby_scene.unity");
        }

        [MenuItem("Tools/Scenes/Game")]
        public static void OpenGameScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/game_scene.unity");
        }

        [MenuItem("Tools/Scenes/Testing")]
        public static void OpenTestingScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/testing_scene.unity");
        }
    }
}