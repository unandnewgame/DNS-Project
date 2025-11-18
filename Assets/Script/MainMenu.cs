using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;    // panel menu utama
    public GameObject optionsPanel;     // panel opsi

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);  // matikan menu
        optionsPanel.SetActive(true);    // hidupkan options
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);   // matikan options
        mainMenuPanel.SetActive(true);   // hidupkan menu
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
