using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [Header("Player Ready Icons")]
    public Image m_P1Ready;
    public Image m_P2Ready;

    [Header("Player One Health")]
    public Image m_P1_Health_1;
    public Image m_P1_Health_2;
    public Image m_P1_Health_3;
    public Image m_P1_Health_4;
    public Image m_P1_Health_5;

    [Header("Player Two Health")]
    public Image m_P2_Health_1;
    public Image m_P2_Health_2;
    public Image m_P2_Health_3;
    public Image m_P2_Health_4;
    public Image m_P2_Health_5;

    [Header("Player One Rounds")]
    public Image m_P1_Round_1;
    public Image m_P1_Round_2;
    public Image m_P1_Round_3;

    [Header("Player Two Rounds")]
    public Image m_P2_Round_1;
    public Image m_P2_Round_2;
    public Image m_P2_Round_3;

    [Header("Start Round Panel")]
    public GameObject m_MainPanel;
    public Text m_RoundText;

    [Header("Victory Panel")]
    public GameObject m_VictoryPanel;
    public Text m_VictoryText;

    [Header("Additional Setting")]
    public bool m_DisableHealthUI = false;

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

    public void Initialize(PlayerController P1, PlayerController P2, int roundCount)
    {
        m_PlayerOne = P1;
        m_PlayerTwo = P2;

        m_PlayerOne.GetHealthComponent().m_OnPlayerDamaged += OnPlayerDamaged;
        m_PlayerTwo.GetHealthComponent().m_OnPlayerDamaged += OnPlayerDamaged;

        m_MainPanel.GetComponent<Animator>().SetTrigger("StartRound");
        m_RoundText.text = "Round " + roundCount;

        if (!m_DisableHealthUI)
        {
            //set all health to visible on initialize
            m_P1_Health_1.gameObject.SetActive(true);
            m_P1_Health_2.gameObject.SetActive(true);
            m_P1_Health_3.gameObject.SetActive(true);
            m_P1_Health_4.gameObject.SetActive(true);
            m_P1_Health_5.gameObject.SetActive(true);

            m_P2_Health_1.gameObject.SetActive(true);
            m_P2_Health_2.gameObject.SetActive(true);
            m_P2_Health_3.gameObject.SetActive(true);
            m_P2_Health_4.gameObject.SetActive(true);
            m_P2_Health_5.gameObject.SetActive(true);
        }
        
    }

    public void UpdateRoundScore(int p1RoundsWon, int p2RoundsWon)
    {
        switch (p1RoundsWon)
        {
            case 0:
                m_P1_Round_1.gameObject.SetActive(false);
                m_P1_Round_2.gameObject.SetActive(false);
                m_P1_Round_3.gameObject.SetActive(false);
                break;
            case 1:
                m_P1_Round_1.gameObject.SetActive(true);
                m_P1_Round_2.gameObject.SetActive(false);
                m_P1_Round_3.gameObject.SetActive(false);
                break;
            case 2:
                m_P1_Round_1.gameObject.SetActive(true);
                m_P1_Round_2.gameObject.SetActive(true);
                m_P1_Round_3.gameObject.SetActive(false);
                break;
            case 3:
                m_P1_Round_1.gameObject.SetActive(true);
                m_P1_Round_2.gameObject.SetActive(true);
                m_P1_Round_3.gameObject.SetActive(true);
                break;
            default:
                Debug.LogError("Player One Rounds Not in normal values: " + p1RoundsWon);
                break;
        }

        switch (p2RoundsWon)
        {
            case 0:
                m_P2_Round_1.gameObject.SetActive(false);
                m_P2_Round_2.gameObject.SetActive(false);
                m_P2_Round_3.gameObject.SetActive(false);
                break;
            case 1:
                m_P2_Round_1.gameObject.SetActive(true);
                m_P2_Round_2.gameObject.SetActive(false);
                m_P2_Round_3.gameObject.SetActive(false);
                break;
            case 2:
                m_P2_Round_1.gameObject.SetActive(true);
                m_P2_Round_2.gameObject.SetActive(true);
                m_P2_Round_3.gameObject.SetActive(false);
                break;
            case 3:
                m_P2_Round_1.gameObject.SetActive(true);
                m_P2_Round_2.gameObject.SetActive(true);
                m_P2_Round_3.gameObject.SetActive(true);
                break;
            default:
                Debug.LogError("Player Two Rounds Not in normal values: " + p2RoundsWon);
                break;
        }

    }

    private void OnPlayerDamaged(PlayerController player, float currentHealth)
    {
        //total health is 500
        //dividing by 100 gives correct numbers for divisors

        if (m_DisableHealthUI) { return; }

        int divisor = Mathf.CeilToInt(currentHealth / 100f);

        Image healthBar_1 = (int)player.m_PlayerNum == 0 ? m_P1_Health_1 : m_P2_Health_1;
        Image healthBar_2 = (int)player.m_PlayerNum == 0 ? m_P1_Health_2 : m_P2_Health_2;
        Image healthBar_3 = (int)player.m_PlayerNum == 0 ? m_P1_Health_3 : m_P2_Health_3;
        Image healthBar_4 = (int)player.m_PlayerNum == 0 ? m_P1_Health_4 : m_P2_Health_4;
        Image healthBar_5 = (int)player.m_PlayerNum == 0 ? m_P1_Health_5 : m_P2_Health_5;

        if (divisor <= 0)
        {
            healthBar_1.gameObject.SetActive(false);
            healthBar_2.gameObject.SetActive(false);
            healthBar_3.gameObject.SetActive(false);
            healthBar_4.gameObject.SetActive(false);
            healthBar_5.gameObject.SetActive(false);
        }
        else if (divisor == 1)
        {
            healthBar_1.gameObject.SetActive(true);
            healthBar_2.gameObject.SetActive(false);
            healthBar_3.gameObject.SetActive(false);
            healthBar_4.gameObject.SetActive(false);
            healthBar_5.gameObject.SetActive(false);
        }
        else if (divisor == 2)
        {
            healthBar_1.gameObject.SetActive(true);
            healthBar_2.gameObject.SetActive(true);
            healthBar_3.gameObject.SetActive(false);
            healthBar_4.gameObject.SetActive(false);
            healthBar_5.gameObject.SetActive(false);
        }
        else if (divisor == 3)
        {
            healthBar_1.gameObject.SetActive(true);
            healthBar_2.gameObject.SetActive(true);
            healthBar_3.gameObject.SetActive(true);
            healthBar_4.gameObject.SetActive(false);
            healthBar_5.gameObject.SetActive(false);
        }
        else if (divisor == 4)
        {
            healthBar_1.gameObject.SetActive(true);
            healthBar_2.gameObject.SetActive(true);
            healthBar_3.gameObject.SetActive(true);
            healthBar_4.gameObject.SetActive(true);
            healthBar_5.gameObject.SetActive(false);
        }
    }

    public void SetPlayerReadyStatus(bool isPlayerOne, bool status)
    {
        if (isPlayerOne)
        {
            m_P1Ready.gameObject.SetActive(status);
        }
        else
        {
            m_P2Ready.gameObject.SetActive(status);
        }
    }

    public void SetVictoryCanvasActive(bool isActive)
    {
        m_VictoryPanel.SetActive(isActive);
    }

    public void SetVictoryText(string text)
    {
        m_VictoryText.text = text;
    }

    private void OnDestroy()
    {
        if (m_PlayerOne)
        {
            m_PlayerOne.GetHealthComponent().m_OnPlayerDamaged -= OnPlayerDamaged;
        }

        if (m_PlayerTwo)
        {
            m_PlayerTwo.GetHealthComponent().m_OnPlayerDamaged -= OnPlayerDamaged;
        }
    }
}