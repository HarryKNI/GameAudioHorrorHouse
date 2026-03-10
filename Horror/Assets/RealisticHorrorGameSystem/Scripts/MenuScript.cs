using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Horror_Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
