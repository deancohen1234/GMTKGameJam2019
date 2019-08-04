using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    public GameObject m_MainMenu;
    public GameObject m_TutorialMenu;
    public GameObject m_CreditsMenu;

    public Button m_CreditsBackButton; //in credits
    public Button m_CreditsButton; //in main menu
    public Button m_TutorialBackButton; //in credits
    public Button m_TutorialButton;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenCreditsMenu()
    {
        m_MainMenu.SetActive(false);
        m_CreditsMenu.SetActive(true);

        m_CreditsBackButton.Select();
    }

    public void CloseCreditsMenu()
    {
        m_MainMenu.SetActive(true);
        m_CreditsMenu.SetActive(false);

        m_CreditsButton.Select();
    }

    public void OpenTutorialMenu()
    {
        m_MainMenu.SetActive(false);
        m_TutorialMenu.SetActive(true);

        m_TutorialBackButton.Select();
    }

    public void CloseTutorialMenu()
    {
        m_MainMenu.SetActive(true);
        m_TutorialMenu.SetActive(false);

        m_TutorialButton.Select();
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
