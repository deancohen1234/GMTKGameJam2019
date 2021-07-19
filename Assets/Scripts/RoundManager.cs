using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum WinState {P1Wins, P2Wins, Tie}

public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    public int m_RoundsNeededToWin = 2;
    public float m_RoundMaxDuration = 6000f; //in seconds

    [Header("Scene Objects")]
    public DivineStatue m_DivineStatue;
    public DivineWeapon m_DivineWeapon;
    public LevelMechanicsContainer m_Container;
    public UIHandler m_UIHandler;

    [Header("Player Prefabs")]
    public GameObject m_PlayerOnePrefab;
    public GameObject m_PlayerTwoPrefab;
    public Transform m_PlayerOneStartPosition;
    public Transform m_PlayerTwoStartPosition;

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

    private float m_RoundEndTime;
    private bool m_RoundComplete;

    private int m_PlayerOneRoundWins;
    private int m_PlayerTwoRoundWins;

    private bool m_DEBUG_RestartRound;

    #region Monobehavior Methods
    private void Start()
    {
        InitializeGame();
        StartRound();
    }

    private void Update()
    {
        if (Time.time > m_RoundEndTime)
        {
            OnMatchComplete(WinState.Tie);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            m_DEBUG_RestartRound = true;
            OnRoundComplete();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnMatchComplete(WinState.Tie);
        }
        
    }

    private void OnDestroy()
    {
        if (m_PlayerOne != null)
        {
            m_PlayerOne.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        }

        if (m_PlayerTwo != null)
        {
            m_PlayerTwo.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        }
    }
    #endregion

    #region Game Flow Methods

    private void InitializeGame()
    {
        m_UIHandler.SetVictoryCanvasActive(false);

        m_UIHandler.UpdateRoundScore(m_PlayerOneRoundWins, m_PlayerTwoRoundWins);
        m_UIHandler.SetPlayerReadyStatus(true, false);
        m_UIHandler.SetPlayerReadyStatus(false, false);

        m_DivineStatue.OnGameIntialized();

        m_Container.InitializeContainer(this);
    }

    private void StartRound()
    {
        //Spawn/initialize players
        InitializePlayer(ref m_PlayerOne, m_PlayerOnePrefab, 0);
        InitializePlayer(ref m_PlayerTwo, m_PlayerTwoPrefab, 1);

        //set player spawn locations
        //m_PlayerOne.transform.position = m_PlayerOneStartPosition.position;
        //m_PlayerTwo.transform.position = m_PlayerTwoStartPosition.position;

        m_UIHandler.Initialize(m_PlayerOne, m_PlayerTwo, (m_PlayerOneRoundWins + m_PlayerTwoRoundWins + 1)); //+1 for 0 based

        m_DivineStatue.SetPlayers(m_PlayerOne, m_PlayerTwo);
        m_DivineStatue.SetDivineWeapon(m_DivineWeapon);
        m_DivineStatue.OnRoundStarted();

        //prevents a double pickup of the weapon incase the weapon was left there from the previous round
        m_DivineWeapon.SetWeaponActive(false);

        m_RoundEndTime = Time.time + m_RoundMaxDuration;
    }

    public void OnRoundComplete()
    {
        //update round scoring
        UpdateRoundScore();

        //remove event listeners from players before destroying them
        ClearPlayerEvents();

        //check for game completion
        if (CheckGameCompletion()) { return; }

        //poof away player and then start new round
        PoofAwayLeftoverPlayer();

        //end all level mechanics
        m_Container.EndRound();
    }


    private void OnMatchComplete(WinState state)
    {
        m_UIHandler.SetVictoryCanvasActive(true);

        if (state == WinState.P1Wins)
        {
            //PlayerTwo wins
            m_UIHandler.SetVictoryText("Player One Wins!!!");
        }
        else if (state == WinState.P2Wins)
        {
            //PlayerOne wins
            m_UIHandler.SetVictoryText("Player Two Wins!!!");
        }
        else if (state == WinState.Tie)
        {
            m_UIHandler.SetVictoryText("Tie Game!!!");
        }

        Invoke("LoadArcadeIdleScreen", 5.0f); //after game is complete, load back to start screen
    }
    #endregion

    #region Helper Methods
    //send a REFERENCE of the player
    //instantiates and adds needed events to player then returns object instantiated
    private void InitializePlayer(ref PlayerController player, GameObject playerPrefab, int playerIndex)
    {
        if (player) { Destroy(player.gameObject); player = null; }

        //get start position
        Vector3 startPos = Vector3.zero;
        if (playerIndex == 0)
        {
            startPos = m_PlayerOneStartPosition.position;
        }
        else if (playerIndex == 1)
        {
            startPos = m_PlayerTwoStartPosition.position;
        }

        //instantiate player and spawn at location
        player = Instantiate(playerPrefab, startPos, Quaternion.identity).GetComponent<PlayerController>();

        player.m_OnDeathComplete += OnRoundComplete;
        player.PlayPoofSound();
        player.GetEffectsController().ActivatePoofSystem();
    }

    private void PoofAwayLeftoverPlayer()
    {
        //kill leftover player then start new round
        if (m_PlayerOne.GetHealthComponent().IsDead() == false)
        {
            m_PlayerOne.DisableController();
            m_PlayerOne.PlayPoofSound();
            m_PlayerOne.GetEffectsController().ActivatePoofSystem();
            m_PlayerOne.GetEffectsController().m_CharacterSprite.enabled = false;
        }

        if (m_PlayerTwo.GetHealthComponent().IsDead() == false)
        {
            m_PlayerTwo.DisableController();
            m_PlayerTwo.PlayPoofSound();
            m_PlayerTwo.GetEffectsController().ActivatePoofSystem();
            m_PlayerTwo.GetEffectsController().m_CharacterSprite.enabled = false;
        }

        //start new round
        Invoke("StartRound", 1.5f);
    }

    private void UpdateRoundScore()
    {
        if (m_DEBUG_RestartRound)
        {
            m_DEBUG_RestartRound = false;
        }
        else if (m_PlayerOne.GetHealthComponent().IsDead() && m_PlayerTwo.GetHealthComponent().IsDead())
        {
            //tie, both players score a point
            m_PlayerOneRoundWins++;
            m_PlayerTwoRoundWins++;
        }
        else if (m_PlayerOne.GetHealthComponent().IsDead())
        {
            //PlayerTwo wins
            m_PlayerTwoRoundWins++;
        }
        else if (m_PlayerTwo.GetHealthComponent().IsDead())
        {
            //PlayerOne wins
            m_PlayerOneRoundWins++;
        }

        m_UIHandler.UpdateRoundScore(m_PlayerOneRoundWins, m_PlayerTwoRoundWins);
    }

    private void ClearPlayerEvents()
    {
        m_PlayerOne.m_OnDeathComplete -= OnRoundComplete;
        m_PlayerTwo.m_OnDeathComplete -= OnRoundComplete;
    }

    private bool CheckGameCompletion()
    {
        bool gameIsComplete = false;

        if (m_PlayerOneRoundWins >= m_RoundsNeededToWin && m_PlayerTwoRoundWins >= m_RoundsNeededToWin)
        {
            OnMatchComplete(WinState.Tie);
            gameIsComplete = true;
        }
        else if (m_PlayerOneRoundWins >= m_RoundsNeededToWin)
        {
            OnMatchComplete(WinState.P1Wins);
            gameIsComplete = true;
        }
        else if (m_PlayerTwoRoundWins >= m_RoundsNeededToWin)
        {
            OnMatchComplete(WinState.P2Wins);
            gameIsComplete = true;
        }

        return gameIsComplete;
    }

    public void LoadArcadeIdleScreen()
    {
        SceneManager.LoadScene("ArcadeIdleScreen");
    }
    #endregion

    #region Public Getters
    public PlayerController GetPlayer(int playerIndex)
    {
        if (playerIndex == 0)
        {
            return m_PlayerOne;
        }
        else if (playerIndex == 1)
        {
            return m_PlayerTwo;
        }
        else
        {
            Debug.LogError("Get Player Function Index Not in Range. Index: " + playerIndex);
            return null;
        }
    }

    #endregion
}
