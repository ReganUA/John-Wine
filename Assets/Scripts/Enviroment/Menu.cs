using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit(); 
#endif
    }
}
