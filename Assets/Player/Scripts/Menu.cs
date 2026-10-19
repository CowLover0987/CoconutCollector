using NUnit;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menu;
    public GameObject controlsUI;
    public GameObject pointUI;

    
    void Start()
    {
        controlsUI.SetActive(false);
        pointUI.SetActive(false);
    }

    public void StartGame()
    {
        menu.SetActive(false);
        controlsUI.SetActive(true);
        pointUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        controlsUI.SetActive(false);
        pointUI.SetActive(false);
    }
}
