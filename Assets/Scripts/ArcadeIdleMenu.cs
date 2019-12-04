using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArcadeIdleMenu : MonoBehaviour
{
    public Image m_P1_Ready;
    public Image m_P2_Ready;

    public string m_StageSelectName = "StageSelect";

    private bool m_GameIsReadyToStart;

    private bool m_P1EnteredCoin;
    private bool m_P2EnteredCoin;

    void Update()
    {
        if (Input.GetButtonDown("P1_CoinSlot"))
        {
            OnCoinEntered(true);
        }

        if (Input.GetButtonDown("P2_CoinSlot"))
        {
            OnCoinEntered(false);
        }

        if (m_GameIsReadyToStart)
        {
            if (Input.GetButtonDown("StartGame"))
            {
                Debug.Log("Starting Game...");
                SceneManager.LoadScene(m_StageSelectName);
            }
        }

    }

    private void OnCoinEntered(bool isPlayerOne)
    {
        if (isPlayerOne)
        {
            Debug.Log("P1 Entered Coin");
            m_P1EnteredCoin = true;

            m_P1_Ready.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("P2 Entered Coin");
            m_P2EnteredCoin = true;

            m_P2_Ready.gameObject.SetActive(true);
        }

        m_GameIsReadyToStart = CheckForGameReady();

    }

    private bool CheckForGameReady()
    {
        return m_P1EnteredCoin & m_P2EnteredCoin;
    }
}
