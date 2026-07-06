using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject UI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
        if (Input.GetKeyDown(KeyCode.Space) && UI.activeSelf)
        {
            Quit();
        }
    }
    private void Pause()
    {
        if (UI.activeSelf == false)
        {
            UI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            UI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit(); 
#endif
    }
}