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
    public DivineStatue m_DivineStatue;
    public DivineWeapon m_DivineWeapon;
    public UIHandler m_UIHandler;

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

    private int m_PlayerOneRoundWins;
    private int m_PlayerTwoRoundWins;

    private bool m_IsPlayerOneReady;
    private bool m_IsPlayerTwoReady;
    private bool m_GameIsStarted;

    private bool m_IsGameComplete;
    private bool m_DEBUG_RestartRound;

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_DEBUG_RestartRound = true;
            OnRoundComplete();
        }
        
    }

    private void InitializeGame()
    {
        m_IsGameComplete = false;
        m_GameIsStarted = false;

        m_IsPlayerOneReady = false;
        m_IsPlayerTwoReady = false;

        m_VictoryCanvas.SetActive(false);

        m_UIHandler.UpdateRoundScore(m_PlayerOneRoundWins, m_PlayerTwoRoundWins);
        m_UIHandler.SetPlayerReadyStatus(true, false);
        m_UIHandler.SetPlayerReadyStatus(false, false);

        m_DivineStatue.OnGameIntialized();
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

        m_PlayerOne.PlayPoofEffect();
        m_PlayerTwo.PlayPoofEffect();

        m_PlayerOne.GetEffectsController().ActivatePoofSystem();
        m_PlayerTwo.GetEffectsController().ActivatePoofSystem();

        m_UIHandler.Initialize(m_PlayerOne, m_PlayerTwo, (m_PlayerOneRoundWins + m_PlayerTwoRoundWins + 1)); //+1 for 0 based
        m_DivineStatue.SetPlayers(m_PlayerOne, m_PlayerTwo);
        m_DivineStatue.SetDivineWeapon(m_DivineWeapon);
        m_DivineStatue.OnRoundStarted();

        m_DivineWeapon.gameObject.SetActive(false); //prevents a double pickup of the weapon

    }

    //TODO split this into multiple functions
    public void OnRoundComplete()
    {
        if (m_DEBUG_RestartRound)
        {
            m_DEBUG_RestartRound = false;
        }
        else if (m_PlayerOne.GetHealthComponent().IsDead())
        {
            //PlayerTwo wins
            m_VictoryText.text = "Player Two Takes a Round";
            m_PlayerTwoRoundWins++;
        }
        else
        {
            //PlayerOne wins
            m_VictoryText.text = "Player One Takes a Round";
            m_PlayerOneRoundWins++;
        }

        m_UIHandler.UpdateRoundScore(m_PlayerOneRoundWins, m_PlayerTwoRoundWins);

        m_PlayerOne.m_OnDeathComplete -= OnRoundComplete;
        m_PlayerTwo.m_OnDeathComplete -= OnRoundComplete;


        if (m_PlayerOneRoundWins >= m_RoundsNeededToWin)
        {
            //End Game
            OnMatchComplete(true);
            return;
        }
        else if (m_PlayerTwoRoundWins >= m_RoundsNeededToWin)
        {
            OnMatchComplete(false);
            return;
        }

        //poof away player and then start new round
        PoofAwayLeftoverPlayer();
    }

    private void CheckPlayerReadiness()
    {
        bool playerOnePressedConfirm = false;
        bool playerTwoPressedConfirm = false;


        playerOnePressedConfirm = ApplicationSettings.m_Singleton.m_InputManager.IsConfirmButtonPressedDown(true);
        playerTwoPressedConfirm = ApplicationSettings.m_Singleton.m_InputManager.IsConfirmButtonPressedDown(false);

        if (playerOnePressedConfirm)
        {
            m_IsPlayerOneReady = true;
            m_UIHandler.SetPlayerReadyStatus(true, true);
        }

        if (playerTwoPressedConfirm)
        {
            m_IsPlayerTwoReady = true;
            m_UIHandler.SetPlayerReadyStatus(false, true);
        }

        if (m_IsPlayerOneReady && m_IsPlayerTwoReady)
        {
            //start game
            m_GameIsStarted = true;
            StartRound();
        }
    }

    private void PoofAwayLeftoverPlayer()
    {
        //kill leftover player then start new round
        if (m_PlayerOne.GetHealthComponent().IsDead() == false)
        {
            m_PlayerOne.DisableAllMovement();
            m_PlayerOne.PlayPoofEffect();
            m_PlayerOne.GetEffectsController().ActivatePoofSystem();
            m_PlayerOne.GetEffectsController().m_CharacterSprite.enabled = false;
        }

        if (m_PlayerTwo.GetHealthComponent().IsDead() == false)
        {
            m_PlayerTwo.DisableAllMovement();
            m_PlayerTwo.PlayPoofEffect();
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

        Invoke("LoadArcadeIdleScreen", 3.0f); //after game is complete, load back to start screen
    }

    private void LoadArcadeIdleScreen()
    {
        SceneManager.LoadScene("ArcadeIdleScreen");
    }

    private bool IsMatchComplete()
    {
        return false;
    }

    private void OnDestroy()
    {
        m_PlayerOne.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        m_PlayerTwo.GetHealthComponent().m_OnDeath -= OnRoundComplete;
    }
}
