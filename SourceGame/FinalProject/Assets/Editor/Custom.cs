

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Custom : EditorWindow
{
    [MenuItem("OpenScene/MainMenu", false, 1)]
    public static void Menu()
    {
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");
    }
    [MenuItem("OpenScene/OldCity", false, 2)]
    public static void OldCity()
    {
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/Scenes/OldCity.unity");
    }
    [MenuItem("OpenScene/Indus1", false, 3)]
    public static void Indus1()
    {
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/Scenes/Industry1.unity");
    }
    [MenuItem("OpenScene/Indus2", false, 4)]
    public static void Indus2()
    {
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/Scenes/Industry2.unity");
    }
    [MenuItem("OpenScene/Offline", false, 5)]
    public static void Offline()
    {
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/Scenes/CampaignOffline.unity");
    }
}
