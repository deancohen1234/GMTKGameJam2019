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
    public EvilMan m_EvilMan;
    public MegaWeapon m_MegaWeapon;
    public UIHandler m_UIHandler;
    public AudioSource m_Music;

    [Header("Player Prefabs")]
    public GameObject m_PlayerOnePrefab;
    public GameObject m_PlayerTwoPrefab;
    public Transform m_PlayerOneStartPosition;
    public Transform m_PlayerTwoStartPosition;

    [Header("UI References")]
    public GameObject m_StartRoundScreen;
    public GameObject m_VictoryCanvas;
    public Text m_VictoryText;

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

    private bool m_RoundComplete;

    private int m_Player1RoundWins;
    private int m_Player2RoundWins;

    private bool m_IsGameComplete;

    private void Awake()
    {
        
    }

    private void Start()
    {
        InitializeGame();

        StartRound();
    }

    private void Update()
    {

        if (m_IsGameComplete || true)
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
        
    }

    //TODO split this into multiple functions
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

        m_UIHandler.UpdateRoundScore(m_Player1RoundWins, m_Player2RoundWins);

        m_PlayerOne.m_OnDeathComplete -= OnRoundComplete;
        m_PlayerTwo.m_OnDeathComplete -= OnRoundComplete;


        if (m_Player1RoundWins >= m_RoundsNeededToWin)
        {
            //End Game
            OnMatchComplete(true);
            return;
        }
        else if (m_Player2RoundWins >= m_RoundsNeededToWin)
        {
            OnMatchComplete(false);
            return;
        }

        //poof away player and then start new round
        PoofAwayLeftoverPlayer();
    }

    private void PoofAwayLeftoverPlayer()
    {
        //kill leftover player then start new round
        if (m_PlayerOne.GetHealthComponent().IsDead() == false)
        {
            m_PlayerOne.DisableAllMovement();
            m_PlayerOne.GetEffectsController().ActivatePoofSystem();
            m_PlayerOne.GetEffectsController().m_CharacterSprite.enabled = false;
        }

        if (m_PlayerTwo.GetHealthComponent().IsDead() == false)
        {
            m_PlayerTwo.DisableAllMovement();
            m_PlayerTwo.GetEffectsController().ActivatePoofSystem();
            m_PlayerTwo.GetEffectsController().m_CharacterSprite.enabled = false;
        }

        //give .5 second gap to finish particle effect
        Invoke("StartRound", 1.5f);
        //start new round
    }

    private void OnMatchComplete(bool playerOneWon)
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

        m_IsGameComplete = true;
    }

    private bool IsMatchComplete()
    {
        return false;
    }

    private void InitializeGame()
    {
        m_IsGameComplete = false;
        m_VictoryCanvas.SetActive(false);

        m_UIHandler.UpdateRoundScore(m_Player1RoundWins, m_Player2RoundWins);

        m_EvilMan.m_OnRoundSlamFinished += OnSlamFinished;
    }

    private void StartRound()
    {
        //Spawn players

        if (m_PlayerOne) { Destroy(m_PlayerOne.gameObject); m_PlayerOne = null; }
        if (m_PlayerTwo) { Destroy(m_PlayerTwo.gameObject); m_PlayerTwo = null; }

        m_PlayerOne = Instantiate(m_PlayerOnePrefab).GetComponent<PlayerController>();
        m_PlayerTwo = Instantiate(m_PlayerTwoPrefab).GetComponent<PlayerController>();

        m_PlayerOne.m_OnDeathComplete += OnRoundComplete;
        m_PlayerTwo.m_OnDeathComplete += OnRoundComplete;

        m_PlayerOne.GetEffectsController().ActivatePoofSystem();
        m_PlayerTwo.GetEffectsController().ActivatePoofSystem();

        m_UIHandler.Initialize(m_PlayerOne, m_PlayerTwo, (m_Player1RoundWins + m_Player2RoundWins + 1)); //+1 for 0 based
        m_EvilMan.SetPlayers(m_PlayerOne, m_PlayerTwo);

        Invoke("DelayedSlam", 3.0f);
    }

    private void DelayedSlam()
    {
        m_EvilMan.StartSlam(SlamType.RoundStarting);
    }

    private void OnSlamFinished()
    {
        Debug.Log("Slam Finished");

        m_MegaWeapon.RandomizeLocation();
        m_Music.Play();
    }

    private void OnDestroy()
    {
        m_PlayerOne.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        m_PlayerTwo.GetHealthComponent().m_OnDeath -= OnRoundComplete;
    }
}
