using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_DEBUG_RestartRound = true;
            OnRoundComplete();
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
    }

    private void StartRound()
    {
        //Spawn/initialize players
        InitializePlayer(ref m_PlayerOne, m_PlayerOnePrefab);
        InitializePlayer(ref m_PlayerTwo, m_PlayerTwoPrefab);

        m_UIHandler.Initialize(m_PlayerOne, m_PlayerTwo, (m_PlayerOneRoundWins + m_PlayerTwoRoundWins + 1)); //+1 for 0 based

        m_DivineStatue.SetPlayers(m_PlayerOne, m_PlayerTwo);
        m_DivineStatue.SetDivineWeapon(m_DivineWeapon);
        m_DivineStatue.OnRoundStarted();

        //prevents a double pickup of the weapon incase the weapon was left there from the previous round
        m_DivineWeapon.gameObject.SetActive(false); 
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
    }


    private void OnMatchComplete(bool playerOneWon)
    {
        m_UIHandler.SetVictoryCanvasActive(true);

        if (m_PlayerOne.GetHealthComponent().IsDead())
        {
            //PlayerTwo wins
            m_UIHandler.SetVictoryText("Player Two Wins!!!");
        }
        else
        {
            //PlayerOne wins
            m_UIHandler.SetVictoryText("Player One Wins!!!");
        }

        Invoke("LoadArcadeIdleScreen", 5.0f); //after game is complete, load back to start screen
    }
    #endregion

    #region Helper Methods
    //send a REFERENCE of the player
    //instantiates and adds needed events to player then returns object instantiated
    private void InitializePlayer(ref PlayerController player, GameObject playerPrefab)
    {
        if (player) { Destroy(player.gameObject); player = null; }

        player = Instantiate(playerPrefab).GetComponent<PlayerController>();
        player.m_OnDeathComplete += OnRoundComplete;
        player.PlayPoofSound();
        player.GetEffectsController().ActivatePoofSystem();
    }

    private void PoofAwayLeftoverPlayer()
    {
        //kill leftover player then start new round
        if (m_PlayerOne.GetHealthComponent().IsDead() == false)
        {
            m_PlayerOne.DisableAllMovement();
            m_PlayerOne.PlayPoofSound();
            m_PlayerOne.GetEffectsController().ActivatePoofSystem();
            m_PlayerOne.GetEffectsController().m_CharacterSprite.enabled = false;
        }

        if (m_PlayerTwo.GetHealthComponent().IsDead() == false)
        {
            m_PlayerTwo.DisableAllMovement();
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
        else if (m_PlayerOne.GetHealthComponent().IsDead())
        {
            //PlayerTwo wins
            m_PlayerTwoRoundWins++;
        }
        else
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

        if (m_PlayerOneRoundWins >= m_RoundsNeededToWin)
        {
            //End Game
            OnMatchComplete(true);
            gameIsComplete = true;
        }
        else if (m_PlayerTwoRoundWins >= m_RoundsNeededToWin)
        {
            OnMatchComplete(false);
            gameIsComplete = true;
        }

        return gameIsComplete;
    }

    private void LoadArcadeIdleScreen()
    {
        SceneManager.LoadScene("ArcadeIdleScreen");
    }
    #endregion
}
