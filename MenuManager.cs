using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Reference to your AR scene (set this in the inspector)
    public string ARSceneName = "Showroom"; // Change to your AR scene name

    // Play button functionality
    public void OnPlayButtonClicked()
    {
        // Load the AR scene
        SceneManager.LoadScene(ARSceneName);
    }

    // Quit button functionality
    public void OnQuitButtonClicked()
    {
        // Quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}