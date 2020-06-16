using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Video;


public class ArcadeIdleMenu : MonoBehaviour
{
    public Image m_CoinInsertedImage;

    public GameObject m_CoinInsertedText;
    public GameObject m_InsertCoinText;

    public GameObject m_SelectMenu;
    public bool menuBool = false;

    public Button vsSelect;
    public Button tutSelect;
    public Button creditSelect;

    public Image m_P1_Ready;
    public Image m_P2_Ready;

    public Text m_CreditText;

    [Header("Video Settings")]
    public VideoPlayer m_VideoPlayer;
    public RawImage m_VideoCanvas;
    public Animator m_ScreenFade;
    public float m_IdleTimeToVideo = 10f; //in seconds

    public string m_StageSelectName = "StageSelect";

    private bool m_GameIsReadyToStart;

    private bool m_PlayerOneReady;
    private bool m_PlayerTwoReady;
    private bool m_VideoIsPlaying;

    private void Start()
    {
        m_ScreenFade.SetTrigger("TriggerFade");
        m_CreditText.text = "Credits: " + CreditsManager.m_Singleton.GetNumCredits().ToString();

        menuBool = false;
        if(menuBool == false)
        {
            m_SelectMenu.gameObject.SetActive(false);
        }

        if(CreditsManager.m_Singleton.GetNumCredits() == 0)
        {
            m_InsertCoinText.gameObject.SetActive(true);
        }
        else
        {
            m_CoinInsertedText.gameObject.SetActive(true);
        }

        InvokeRepeating("CheckForVideoPlay", 0, 1.0f);

        ApplicationSettings.m_Singleton.m_InputManager.ForceInput();
    }

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_VideoPlayer.frame = 2200;
        }

        if (ApplicationSettings.m_Singleton.m_InputManager.IsCoinButtonPressedDown())
        {
            OnCoinEntered();
        }

        if (CanGameStart())
        {
            if (ApplicationSettings.m_Singleton.m_InputManager.IsStartButtonPressedDown())
            {
                //StartGame();
                menuBool = true;
                m_CoinInsertedText.gameObject.SetActive(false);
                m_SelectMenu.gameObject.SetActive(true);
                vsSelect.Select();
            }
        }

    }

    public void vsClick()
    {
        StartGame();
    }

    private bool CanGameStart()
    {
        bool check = (CreditsManager.m_Singleton.GetNumCredits() > 0);
        return check;
    }

    private void OnCoinEntered()
    {
        m_InsertCoinText.gameObject.SetActive(false);

        if(menuBool == false)
        {
            m_CoinInsertedText.gameObject.SetActive(true);
        }

        m_GameIsReadyToStart = true;

        int numCredits = CreditsManager.m_Singleton.ModifyCredits(1);
        m_CreditText.text = "Credits: " + numCredits.ToString();
    }

    private bool CheckForGameReady()
    {
        return m_PlayerOneReady & m_PlayerTwoReady;
    }

    private void StartGame()
    {
        Debug.Log("Starting Game...");

        CreditsManager.m_Singleton.ModifyCredits(-1);

        SceneManager.LoadScene(m_StageSelectName);
    }

    private void CheckForVideoPlay()
    {
        float lastInputTime = ApplicationSettings.m_Singleton.m_InputManager.GetLastInputTime();

        if (m_VideoIsPlaying == false)
        {
            if (Time.time - lastInputTime >= m_IdleTimeToVideo)
            {
                SwapToVideoMode();
            }
        }
        else
        {
            if (m_VideoPlayer.frame >= 150)
            {
                Debug.Log(Time.time - lastInputTime);
                if (Time.time - lastInputTime <= 5.0f || (ulong)m_VideoPlayer.frame + 1 >= m_VideoPlayer.frameCount)
                {
                    ApplicationSettings.m_Singleton.m_InputManager.ForceInput();
                    SwapToMainScreen();
                }
            }

        }
    }

    public void SwapToVideoMode()
    {
        m_VideoIsPlaying = true;
        m_ScreenFade.SetTrigger("TriggerFade");
        Invoke("DelaySwapVideoMode", 3.0f);
    }

    private void DelaySwapVideoMode()
    {
        m_ScreenFade.SetTrigger("TriggerFade");
        m_VideoCanvas.color = Color.white;
        m_VideoPlayer.time = 0;
        m_VideoPlayer.Play();
    }

    private void SwapToMainScreen()
    {
        Debug.Log("Swapping to Main Mode");

        m_VideoIsPlaying = false;
        Color noAlphaWhite = new Color(1, 1, 1, 0);
        m_VideoCanvas.color = noAlphaWhite;

        m_VideoPlayer.Stop();
    }
}
