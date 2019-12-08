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

    private bool m_CoinIsEntered;

    void Update()
    {
        Debug.Log("Coin Input: " + GetCoinInsertedInput());
        Debug.Log("Start Game Input: " + GetGameStartInput());

        /*if (Input.GetButtonDown("P1_CoinSlot"))
        {
            OnCoinEntered();
        }

        if (Input.GetButtonDown("P2_CoinSlot"))
        {
            OnCoinEntered();
        }*/

        if (Input.GetButtonDown(ApplicationSettings.m_GlobalInput.P1_ActionButton))
        {
            OnPlayerReady(true);
        }

        if (Input.GetButtonDown(ApplicationSettings.m_GlobalInput.P2_ActionButton))
        {
            OnPlayerReady(false);
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

    private void OnCoinEntered()
    {
        m_CoinIsEntered = true;

        m_CoinInsertedImage.gameObject.SetActive(true);
    }

    private void OnPlayerReady(bool isPlayerOne)
    {
        if (isPlayerOne)
        {
            m_PlayerOneReady = true;
            m_P1_Ready.gameObject.SetActive(true);
        }
        else
        {
            m_PlayerTwoReady = true;
            m_P2_Ready.gameObject.SetActive(true);
        }

        m_GameIsReadyToStart = CheckForGameReady();
    }

    private bool CheckForGameReady()
    {
        return m_PlayerOneReady & m_PlayerTwoReady;
    }

    //Arcade special inputs
    public bool GetCoinInsertedInput()
    {
        bool check = false;

        float p2CoinInput = Input.GetAxis("P2_CoinSlot");
        float p1_startGameInput = Input.GetAxis("P1_StartGame_Input");

        Debug.Log("In1: " + p2CoinInput + "\nIn2: " + p1_startGameInput);

        p2CoinInput = Mathf.Round(p2CoinInput);
        p1_startGameInput = Mathf.Round(p1_startGameInput);

        if (DeanUtils.IsAlmostEqual(p2CoinInput, -1f, 0.01f))
        {
            check = true;
        }

        if (DeanUtils.IsAlmostEqual(p1_startGameInput, -1f, 0.01f))
        {
            check = true;
        }

        return check;
    }

    public bool GetGameStartInput()
    {
        bool check = false;

        float p1_startGameInput = Input.GetAxis("P2_CoinSlot");
        p1_startGameInput = Mathf.Round(p1_startGameInput);

        if (DeanUtils.IsAlmostEqual(p1_startGameInput, 1f, 0.01f))
        {
            check = true;
        }

        return check;
    }
}
