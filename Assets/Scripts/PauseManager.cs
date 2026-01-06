using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;
    public LoadSceneButton button;
    public InputActionAsset action;
    void Update()
    {
        // Check for ESC key press
        if (action.FindAction("Pause").triggered)
        {
            button.LoadScene();
        }
    }

    
}
