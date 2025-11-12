using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject mainmenuPanel;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            settingsPanel.SetActive(true);
          
        }
    }

    void Start()
    {
        mainmenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        mainmenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainmenuPanel.SetActive(true);
    }
}
