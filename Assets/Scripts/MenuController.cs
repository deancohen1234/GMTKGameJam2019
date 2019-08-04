using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject m_MainMenu;
    public GameObject m_CreditsMenu;

    public Button m_BackButton; //in credits
    public Button m_CreditsButton; //in main menu

    public void OpenCreditsMenu()
    {
        m_MainMenu.SetActive(false);
        m_CreditsMenu.SetActive(true);

        m_BackButton.Select();
    }

    public void CloseCreditsMenu()
    {
        m_MainMenu.SetActive(true);
        m_CreditsMenu.SetActive(false);

        m_CreditsButton.Select();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Arena");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
