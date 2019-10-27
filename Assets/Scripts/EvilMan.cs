using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SlamType {Impatient, RoundStarting}

public class EvilMan : MonoBehaviour
{
    public AudioClip m_Slam;

    public Action m_OnRoundSlamFinished;
    public Action m_OnImpatientSlamFinished;
    public float m_ImpatienceTime = 20f; //in seconds

    private SlamType m_LastSlamType;
    private AudioSource m_AudioSource;
    private Animator m_Animator;

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

    private CameraShake m_CameraShake;
    private RippleEffect m_RippleEffect;

    private float m_LastSlamTime;
    // Start is called before the first frame update
    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();

        m_CameraShake = FindObjectOfType<CameraShake>();
        m_RippleEffect = FindObjectOfType<RippleEffect>();
    }

    private void Update()
    {
        if (Time.time - m_LastSlamTime >= m_ImpatienceTime)
        {
            //he's impatient, he gonna slam
            StartSlam(SlamType.Impatient);
        }
    }

    private void OnEnable()
    {
        GetComponent<AnimationEventRouter>().m_OnAnimationComplete += OnSlamFinished;

    }
    private void OnDisable()
    {
        GetComponent<AnimationEventRouter>().m_OnAnimationComplete -= OnSlamFinished;
    }

    public void SetPlayers(PlayerController p1, PlayerController p2)
    {
        m_PlayerOne = p1;
        m_PlayerTwo = p2;
    }

    public void StartSlam(SlamType slamType)
    {
        m_Animator.SetTrigger("SlamDown");

        m_LastSlamTime = Time.time;
        m_LastSlamType = slamType;
    }

    public void OnSlamFinished()
    {
        m_AudioSource.clip = m_Slam;
        m_AudioSource.Play();

        if (m_LastSlamType == SlamType.RoundStarting)
        {
            RoundSlamComplete();
            m_OnRoundSlamFinished?.Invoke();
        }
        else if (m_LastSlamType == SlamType.Impatient)
        {
            ImpatientSlamComplete();
            m_OnImpatientSlamFinished?.Invoke();
        }
    }

    public void OnPlayerDamaged()
    {
        m_LastSlamTime += 10f; //give ten more seconds before Slqth'thiss slams again when player is damaged
    }
    
    private void RoundSlamComplete()
    {
        m_CameraShake.AddTrauma(2.0f);
        m_RippleEffect.ActivateRipple(transform.position);
    }

    private void ImpatientSlamComplete()
    {
        m_CameraShake.AddTrauma(2.0f);
        m_RippleEffect.ActivateRipple(transform.position);

        m_PlayerOne.DropWeapon();
        m_PlayerTwo.DropWeapon();
    }
}
