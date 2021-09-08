using UnityEditor;
using UnityEngine;

public static class Utils
{
    #region Pause
    [MenuItem("Tools/<<Force Pause>> #Z", false, -100)]
    public static void ForcePause()
    {
        EditorApplication.isPaused = !EditorApplication.isPaused;
    }
    #endregion
    #region ClearPlayerPrefs
    [MenuItem("Tools/<<Clear PlayerPrefs>> %#Z", false, -100)]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs cleared");
    }
    #endregion
}
