using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [Header("Menu Root")]
    [SerializeField] private GameObject optionsMenu;

    [Header("Category ScrollViews")]
    [SerializeField] private GameObject displayScrollView;
    [SerializeField] private GameObject audioScrollView;
    [SerializeField] private GameObject controlsScrollView;

    private void Start()
    {
        CloseOptions();
    }

    // ---------- Menu ----------
    public void OpenOptions()
    {
        optionsMenu.SetActive(true);
        ShowDisplay(); // default category
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
    }

    // ---------- Categories ----------
    public void ShowDisplay()
    {
        displayScrollView.SetActive(true);
        audioScrollView.SetActive(false);
        controlsScrollView.SetActive(false);
    }

    public void ShowAudio()
    {
        displayScrollView.SetActive(false);
        audioScrollView.SetActive(true);
        controlsScrollView.SetActive(false);
    }

    public void ShowControls()
    {
        displayScrollView.SetActive(false);
        audioScrollView.SetActive(false);
        controlsScrollView.SetActive(true);
    }
}
