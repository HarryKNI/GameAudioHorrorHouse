using UnityEngine;
using UnityEngine.SceneManagement;



public class MenuScript : MonoBehaviour
{

    public GameObject OptionsPanel;


    public void StartGame()
    {
        SceneManager.LoadScene("Horror_Scene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenOptionsPanel()
    {
        if (OptionsPanel != null)
        {
            OptionsPanel.SetActive(true);
        }
    }

    public void CloseOptionsPanel()
    {
        if (OptionsPanel != null)
        {
            OptionsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
