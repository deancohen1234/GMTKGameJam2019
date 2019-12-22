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

    public string m_StageSelectName = "StageSelect";

    private bool m_GameIsReadyToStart;

    private bool m_PlayerOneReady;
    private bool m_PlayerTwoReady;

    void Update()
    {

        if (ApplicationSettings.m_Singleton.m_InputManager.IsCoinButtonPressedDown())
        {
            OnCoinEntered();
        }

        if (m_GameIsReadyToStart)
        {
            if (ApplicationSettings.m_Singleton.m_InputManager.IsStartButtonPressedDown())
            {
                Debug.Log("Starting Game...");
                SceneManager.LoadScene(m_StageSelectName);
            }       
        }

    }

    private void OnCoinEntered()
    {
        m_CoinInsertedImage.gameObject.SetActive(true);

        m_GameIsReadyToStart = true;
    }

    private bool CheckForGameReady()
    {
        return m_PlayerOneReady & m_PlayerTwoReady;
    }
}
