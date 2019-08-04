using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RoundManager : MonoBehaviour
{
    [Header("Scene Objects")]
    public GameObject m_EvilMan;
    public MegaWeapon m_MegaWeapon;
    public AudioSource m_Music;

    [Header("Player References")]
    public PlayerController PlayerOne;
    public PlayerController PlayerTwo;

    [Header("UI References")]
    public GameObject m_VictoryCanvas;
    public Text m_VictoryText;

    private bool m_RoundComplete;
    private CameraShake m_CameraShake;
    private RippleEffect m_RippleEffect;

    private void Awake()
    {
        m_CameraShake = FindObjectOfType<CameraShake>();
        m_RippleEffect = FindObjectOfType<RippleEffect>();
    }

    private void Start()
    {
        m_VictoryCanvas.SetActive(false);

        PlayerOne.GetHealthComponent().m_OnDeath += OnRoundComplete;
        PlayerTwo.GetHealthComponent().m_OnDeath += OnRoundComplete;

        m_EvilMan.GetComponent<Animator>().SetTrigger("SlamDown");

        m_EvilMan.GetComponent<AnimationEventRouter>().m_OnAnimationComplete += OnSlamFinished;

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
        PlayerOne.GetHealthComponent().m_OnDeath -= OnRoundComplete;
        PlayerTwo.GetHealthComponent().m_OnDeath -= OnRoundComplete;
    }
}
