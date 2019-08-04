using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RoundManager : MonoBehaviour
{
    public PlayerController PlayerOne;
    public PlayerController PlayerTwo;

    public GameObject m_VictoryCanvas;
    public Text m_VictoryText;

    private bool m_RoundComplete;

    private void Start()
    {
        m_VictoryCanvas.SetActive(false);

        PlayerOne.GetHealthComponent().m_OnDeath += OnRoundComplete;
        PlayerTwo.GetHealthComponent().m_OnDeath += OnRoundComplete;
    }

    private void Update()
    {
        var gamePads = Gamepad.all;

        for (int i = 0; i < gamePads.Count; i++)
        {
            if (gamePads[i].aButton.isPressed)
            {
                //restart scene
                SceneManager.LoadScene("Arena");
            }
        }
    }

    public void OnRoundComplete()
    {
        m_VictoryCanvas.SetActive(true);

        if (PlayerOne.GetHealthComponent().IsDead())
        {
            //PlayerTwo wins
            m_VictoryText.text = "Player Two Wins!!!";
        }
        else
        {
            //PlayerOne wins
            m_VictoryText.text = "Player One Wins!!!";

        }

        m_RoundComplete = true;
    }

    private void OnDestroy()
    {
        PlayerOne.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        PlayerTwo.GetHealthComponent().m_OnDeath -= OnRoundComplete;
    }
}
