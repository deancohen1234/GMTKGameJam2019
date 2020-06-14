using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ArcadeIdleMenu : MonoBehaviour
{
    public Image m_CoinInsertedImage;

    public GameObject m_CoinInsertedText;
    public GameObject m_InsertCoinText;

    public GameObject m_SelectMenu;
    public bool menuBool = false;

    public Button vsSelect;
    public Button tutSelect;
    public Button creditSelect;

    public Image m_P1_Ready;
    public Image m_P2_Ready;

    public Text m_CreditText;

    public string m_StageSelectName = "StageSelect";

    private bool m_GameIsReadyToStart;

    private bool m_PlayerOneReady;
    private bool m_PlayerTwoReady;

    private void Start()
    {
        m_CreditText.text = "Credits: " + CreditsManager.m_Singleton.GetNumCredits().ToString();

        menuBool = false;
        if(menuBool == false)
        {
            m_SelectMenu.gameObject.SetActive(false);
        }

        if(CreditsManager.m_Singleton.GetNumCredits() == 0)
        {
            m_InsertCoinText.gameObject.SetActive(true);
        }
        else
        {
            m_CoinInsertedText.gameObject.SetActive(true);
        }
    }

    void Update()
    {

        if (ApplicationSettings.m_Singleton.m_InputManager.IsCoinButtonPressedDown())
        {
            OnCoinEntered();
        }

        if (CanGameStart())
        {
            if (ApplicationSettings.m_Singleton.m_InputManager.IsStartButtonPressedDown())
            {
                //StartGame();
                menuBool = true;
                m_CoinInsertedText.gameObject.SetActive(false);
                m_SelectMenu.gameObject.SetActive(true);
                vsSelect.Select();
            }
        }

    }

    public void vsClick()
    {
        StartGame();
    }

    private bool CanGameStart()
    {
        bool check = (CreditsManager.m_Singleton.GetNumCredits() > 0);
        return check;
    }

    private void OnCoinEntered()
    {
        m_InsertCoinText.gameObject.SetActive(false);

        if(menuBool == false)
        {
            m_CoinInsertedText.gameObject.SetActive(true);
        }

        m_GameIsReadyToStart = true;

        int numCredits = CreditsManager.m_Singleton.ModifyCredits(1);
        m_CreditText.text = "Credits: " + numCredits.ToString();
    }

    private bool CheckForGameReady()
    {
        return m_PlayerOneReady & m_PlayerTwoReady;
    }

    private void StartGame()
    {
        Debug.Log("Starting Game...");

        CreditsManager.m_Singleton.ModifyCredits(-1);

        SceneManager.LoadScene(m_StageSelectName);
    }
}
