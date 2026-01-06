using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public bool isPaused = false;
    public LoadSceneButton button;
    public InputActionAsset action;
    public GameObject panel;

    void Update()
    {
        // Check for ESC key press
        if (action.FindAction("Pause").triggered)
        {
            isPaused=!isPaused;
            
        }
        if (isPaused)
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1.0f;

        }
    }

    
}
