using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    public int m_RoundsNeededToWin = 2;

    [Header("Scene Objects")]
    public GameObject m_EvilMan;
    public MegaWeapon m_MegaWeapon;
    public UIHandler m_UIHandler;
    public AudioSource m_Music;

    [Header("Player Prefabs")]
    public GameObject m_PlayerOnePrefab;
    public GameObject m_PlayerTwoPrefab;
    public Transform m_PlayerOneStartPosition;
    public Transform m_PlayerTwoStartPosition;

    [Header("UI References")]
    public GameObject m_VictoryCanvas;
    public Text m_VictoryText;

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

    private bool m_RoundComplete;
    private CameraShake m_CameraShake;
    private RippleEffect m_RippleEffect;

    private int m_Player1RoundWins;
    private int m_Player2RoundWins;

    private void Awake()
    {
        m_CameraShake = FindObjectOfType<CameraShake>();
        m_RippleEffect = FindObjectOfType<RippleEffect>();
    }

    private void Start()
    {
        InitializeGame();

        StartRound();
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
        if (m_PlayerOne.GetHealthComponent().IsDead())
        {
            //PlayerTwo wins
            m_VictoryText.text = "Player Two Takes a Round";
            m_Player2RoundWins++;
        }
        else
        {
            //PlayerOne wins
            m_VictoryText.text = "Player One Takes a Round";
            m_Player1RoundWins++;
        }

        m_PlayerOne.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        m_PlayerTwo.GetHealthComponent().m_OnDeath -= OnRoundComplete;

        if (m_Player1RoundWins >= m_RoundsNeededToWin || m_Player2RoundWins >= m_RoundsNeededToWin)
        {
            //End Game
            OnMatchComplete();
            return;
        }

        //start new round
        StartRound();

    }

    private bool IsMatchComplete()
    {
        return false;
    }

    private void InitializeGame()
    {
        m_VictoryCanvas.SetActive(false);

        m_EvilMan.GetComponent<Animator>().SetTrigger("SlamDown");

        m_EvilMan.GetComponent<AnimationEventRouter>().m_OnAnimationComplete += OnSlamFinished;
    }

    private void StartRound()
    {
        //Spawn players
        m_PlayerOne = Instantiate(m_PlayerOnePrefab).GetComponent<PlayerController>();
        m_PlayerTwo = Instantiate(m_PlayerTwoPrefab).GetComponent<PlayerController>();

        m_PlayerOne.GetHealthComponent().m_OnDeath += OnRoundComplete;
        m_PlayerTwo.GetHealthComponent().m_OnDeath += OnRoundComplete;

        m_UIHandler.Initialize(m_PlayerOne, m_PlayerTwo);
    }

    private void OnMatchComplete()
    {
        m_VictoryCanvas.SetActive(true);

        if (m_PlayerOne.GetHealthComponent().IsDead())
        {
            //PlayerTwo wins
            m_VictoryText.text = "Player Two Wins!!!";
        }
        else
        {
            //PlayerOne wins
            m_VictoryText.text = "Player One Wins!!!";

        }
    }

    private void OnSlamFinished()
    {
        Debug.Log("Slam Finished");
        m_CameraShake.AddTrauma(2.0f);

        m_RippleEffect.ActivateRipple(m_EvilMan.transform.position);
        m_MegaWeapon.RandomizeLocation();

        m_EvilMan.GetComponent<AudioSource>().Play();

        m_Music.Play();
    }

    private void OnDestroy()
    {
        m_PlayerOne.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        m_PlayerTwo.GetHealthComponent().m_OnDeath -= OnRoundComplete;
    }
}
