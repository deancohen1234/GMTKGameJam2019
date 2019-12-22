using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArcadeIdleMenu : MonoBehaviour
{
    public Image m_CoinInsertedImage;

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
                StartGame();
            }       
        }

    }

    private bool CanGameStart()
    {
        bool check = (CreditsManager.m_Singleton.GetNumCredits() > 0);
        return check;
    }

    private void OnCoinEntered()
    {
        m_CoinInsertedImage.gameObject.SetActive(true);

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

        SceneManager.LoadScene("ArcadeIdleScreen");
    }
}
