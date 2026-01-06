using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName = "Environment";

    public void LoadScene()
    {

        SceneManager.LoadScene(sceneName);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
