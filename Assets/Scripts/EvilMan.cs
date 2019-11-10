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
    public float m_TimeSavedPerAttack = 5f;

    public GameObject m_StalagmitePrefab;
    public int m_NumStalagmites = 2;

    public Transform m_ArenaCenter;
    public Vector3 m_Offset;
    public float m_ArenaWidth;
    public float m_ArenaHeight;

    private SlamType m_LastSlamType;
    private AudioSource m_AudioSource;
    private Animator m_Animator;

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

    private CameraShake m_CameraShake;
    private RippleEffect m_RippleEffect;

    private Stalagmite[] m_Stalagmites;

    private float m_LastSlamTime;
    // Start is called before the first frame update
    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Animator = GetComponent<Animator>();

        m_CameraShake = FindObjectOfType<CameraShake>();
        m_RippleEffect = FindObjectOfType<RippleEffect>();

        m_Stalagmites = new Stalagmite[m_NumStalagmites];

        m_LastSlamTime = Time.time;
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
        if (m_PlayerOne)
        {
            m_PlayerOne.GetHealthComponent().m_OnPlayerDamaged -= OnPlayerDamaged;
            m_PlayerOne = null;
        }
        if (m_PlayerTwo)
        {
            m_PlayerTwo.GetHealthComponent().m_OnPlayerDamaged -= OnPlayerDamaged;
            m_PlayerTwo = null;
        }

        m_PlayerOne = p1;
        m_PlayerTwo = p2;

        m_PlayerOne.GetHealthComponent().m_OnPlayerDamaged += OnPlayerDamaged;
        m_PlayerTwo.GetHealthComponent().m_OnPlayerDamaged += OnPlayerDamaged;

        m_LastSlamTime = Time.time;
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

    public void OnPlayerDamaged(PlayerController player, float currentHealth)
    {
        //give more seconds before Slqth'thiss slams again when player is damaged
        float newTime = Mathf.Clamp(m_LastSlamTime + m_TimeSavedPerAttack, 0, Time.time);
        m_LastSlamTime = newTime;
    }
    
    private void RoundSlamComplete()
    {
        m_CameraShake.AddTrauma(2.0f);
        m_RippleEffect.ActivateRipple(transform.position);

        PlaceStalagmites();
    }

    private void ImpatientSlamComplete()
    {
        m_CameraShake.AddTrauma(2.0f);
        m_RippleEffect.ActivateRipple(transform.position);

        m_PlayerOne.DropWeapon(false);
        m_PlayerTwo.DropWeapon(false);

        PlaceStalagmites();
    }

    private void PlaceStalagmites()
    {
        for (int i = 0; i < m_Stalagmites.Length; i++)
        {
            if (m_Stalagmites[i] != null)
            {
                m_Stalagmites[i].Hide();
            }

            float randomX = UnityEngine.Random.Range(-1.0f, 1.0f) * m_ArenaWidth;
            float randomY = UnityEngine.Random.Range(-1.0f, 1.0f) * m_ArenaHeight;
            Vector3 newPosition = m_ArenaCenter.position + new Vector3(randomX, .25f, randomY) + m_Offset;

            GameObject s = Instantiate(m_StalagmitePrefab);
            s.transform.position = newPosition;

            float degX = UnityEngine.Random.Range(-20, 20f);
            float degY = UnityEngine.Random.Range(-1.0f, 1.0f);
            float degZ = UnityEngine.Random.Range(-20f, 20f);
            s.transform.rotation = Quaternion.Euler(degX, degY, degZ);

            m_Stalagmites[i] = s.GetComponent<Stalagmite>();
        }
        
    }

    private void OnGUI()
    {
        //GUI.Label(new Rect(20, 20, 100, 100), "Time Left: " + (m_ImpatienceTime - (Time.time - m_LastSlamTime)));
    }
}
