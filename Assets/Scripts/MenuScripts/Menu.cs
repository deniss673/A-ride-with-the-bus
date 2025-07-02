using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menuPrefab;
    GameObject menu;
    void Start()
    {
        if (menuPrefab != null)
        {
            menu = Instantiate(menuPrefab);
        }
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

    }

    void Update()
    {
        
    }


    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }



    public void Options()
    {
        SceneManager.LoadScene("Options");
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }


}
