using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SlamType {Impatient, RoundStarting}

public class SldqhThss : DivineStatue
{

    public float m_ImpatienceTime = 20f; //in seconds
    public float m_TimeSavedPerAttack = 5f;

    public Transform m_ArenaCenter;
    public Vector3 m_Offset;
    public float m_ArenaWidth;
    public float m_ArenaHeight;

    [Header("Stalagmites")]
    public GameObject m_StalagmitePrefab;
    public Vector3 m_StalagmiteCheckBoxSize;
    public LayerMask checkBoxLayerMask = ~0;
    public int m_NumStalagmites = 2;

    [Header("Audio Clips")]
    public AudioClip m_Slam;
    public AudioClip[] m_ImpatienceSounds;

    [Header("Aura Settings")]
    public ParticleSystem m_SwishSystem;
    public ParticleSystem m_ImpatienceBurst;
    public AnimationCurve m_AuraIntensityCurve;
    public float m_StartEmission;
    public float m_StartLifetime;
    public Vector3 m_StartOrbitalSpeed;
    public Vector3 m_StartVelocity;
    public float m_StartTrailLength;
    public float m_EndEmission;
    public float m_EndLifetime;
    public Vector3 m_EndOrbitalSpeed;
    public Vector3 m_EndVelocity;
    public float m_EndTrailLength;

    private DivineWeapon m_Weapon;

    private SlamType m_LastSlamType;
    private AudioSource m_AudioSource;
    private Animator m_Animator;

    private PlayerController m_PlayerOne;
    private PlayerController m_PlayerTwo;

    private CameraShake m_CameraShake;
    private RippleEffect m_RippleEffect;

    private Stalagmite[] m_Stalagmites;

    private float m_LastSlamTime;

    private const int MAXSTALAGMITEPOSITIONTESTS = 5;

    #region Monobehavior Methods
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
        UpdateImpatienceStatue();

        if (Time.time - m_LastSlamTime >= m_ImpatienceTime)
        {
            //he's impatient, he gonna slam
            StartSlam(SlamType.Impatient);

            int randomIndex = UnityEngine.Random.Range(0, m_ImpatienceSounds.Length);
            AudioClip randomSound = m_ImpatienceSounds[randomIndex];
            m_AudioSource.clip = randomSound;
            m_AudioSource.Play();
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

    private void OnDestroy()
    {
        //SetMaterialProperties(m_StartOutlineWidth, m_StartBrightness);
    }

    #endregion

    #region Divine Statue Methods
    public override void OnGameIntialized()
    {

    }

    public override void SetPlayers(PlayerController p1, PlayerController p2)
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

    public override void SetDivineWeapon(DivineWeapon weapon)
    {
        m_Weapon = weapon;
    }

    public override void OnRoundStarted()
    {
        Invoke("DelayedSlam", 3.0f);
    }

    public override void OnRoundComplete()
    {
        
    }

    public override void OnGameComplete()
    {
        
    }
    #endregion

    #region Private Methods

    //make statue more angry and fill with more light as impatience grows
    private void UpdateImpatienceStatue()
    {
        //get lerp time for impatience
        //float lerpTime = Mathf.Lerp(m_LastSlamTime, m_LastSlamTime + m_ImpatienceTime, Time.time);
        float time = DeanUtils.Map(Time.time, m_LastSlamTime, m_LastSlamTime + m_ImpatienceTime, 0, 1);
        float lerpTime = m_AuraIntensityCurve.Evaluate(time);

        float emission = Mathf.Lerp(m_StartEmission, m_EndEmission, lerpTime);
        float lifetime = Mathf.Lerp(m_StartLifetime, m_EndLifetime, lerpTime);
        Vector3 orbitalSpeed = Vector3.Lerp(m_StartOrbitalSpeed, m_EndOrbitalSpeed, lerpTime);
        Vector3 velocity = Vector3.Lerp(m_StartVelocity, m_EndVelocity, lerpTime);
        float trailLength = Mathf.Lerp(m_StartTrailLength, m_EndTrailLength, lerpTime);

        SetParticleProperties(emission, lifetime, orbitalSpeed, velocity, trailLength);
    }

    private void DelayedSlam()
    {
        StartSlam(SlamType.RoundStarting);
    }

    private void StartSlam(SlamType slamType)
    {
        if (slamType == SlamType.Impatient)
        {
            m_ImpatienceBurst.Emit(300);
            m_CameraShake.AddTrauma(.99f, .97f);
        }

        m_Animator.SetTrigger("SlamDown");

        m_LastSlamTime = Time.time;
        m_LastSlamType = slamType;
    }

    private void OnSlamFinished()
    {
        m_AudioSource.clip = m_Slam;
        m_AudioSource.Play();

        if (m_LastSlamType == SlamType.RoundStarting)
        {
            RoundSlamComplete();
        }
        else if (m_LastSlamType == SlamType.Impatient)
        {
            ImpatientSlamComplete();
        }
    }

    private void OnPlayerDamaged(PlayerController player, float currentHealth)
    {
        //give more seconds before Slqth'thiss slams again when player is damaged
        float newTime = Mathf.Clamp(m_LastSlamTime + m_TimeSavedPerAttack, 0, Time.time);
        m_LastSlamTime = newTime;
    }
    
    private void RoundSlamComplete()
    {
        m_Weapon.RandomizeLocation();
        MusicController.m_Singleton.Play();

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

            //check to make sure position doesn't have player in it
            Vector3 spawnPosition = FindValidStalagmitePosition();
            //if spawn is 0 then just don't spawn this stalagmite
            if (spawnPosition == Vector3.zero)
            {
                continue;
            }

            GameObject s = Instantiate(m_StalagmitePrefab);
            s.transform.position = FindValidStalagmitePosition();

            float degX = UnityEngine.Random.Range(-20, 20f);
            float degY = UnityEngine.Random.Range(-1.0f, 1.0f);
            float degZ = UnityEngine.Random.Range(-20f, 20f);
            s.transform.rotation = Quaternion.Euler(degX, degY, degZ);

            m_Stalagmites[i] = s.GetComponent<Stalagmite>();
        }
        
    }

    private Vector3 FindValidStalagmitePosition()
    {
        for (int i = 0; i < MAXSTALAGMITEPOSITIONTESTS; i++)
        {
            float randomX = UnityEngine.Random.Range(-1.0f, 1.0f) * m_ArenaWidth;
            float randomY = UnityEngine.Random.Range(-1.0f, 1.0f) * m_ArenaHeight;
            Vector3 newPosition = m_ArenaCenter.position + new Vector3(randomX, .25f, randomY) + m_Offset;

            bool isBlocked = Physics.CheckBox(newPosition, m_StalagmiteCheckBoxSize, Quaternion.identity, checkBoxLayerMask);

            if (!isBlocked)
            {
                return newPosition;
            }
        }

        return Vector3.zero;
        
    }

    private void SetParticleProperties(float emission, float lifetime, Vector3 orbitalSpeed, Vector3 velocity, float trailLength)
    {
        ParticleSystem.MainModule mainModule = m_SwishSystem.main;
        ParticleSystem.EmissionModule emissionModule = m_SwishSystem.emission;
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = m_SwishSystem.velocityOverLifetime;
        ParticleSystem.TrailModule trailModule = m_SwishSystem.trails;

        mainModule.startLifetime = lifetime;
        emissionModule.rateOverTime = emission;

        velocityOverLifetime.x = velocity.x;
        velocityOverLifetime.y = velocity.y;
        velocityOverLifetime.z = velocity.z;

        velocityOverLifetime.orbitalX = orbitalSpeed.x;
        velocityOverLifetime.orbitalY = orbitalSpeed.y;
        velocityOverLifetime.orbitalZ = orbitalSpeed.z;

        trailModule.lifetime = new ParticleSystem.MinMaxCurve(trailLength, (trailLength * 1.20f)); //length and max of 20% more of length
    }

    private void OnGUI()
    {
        //GUI.Label(new Rect(20, 20, 100, 100), "Time Left: " + (m_ImpatienceTime - (Time.time - m_LastSlamTime)));
    }
    #endregion
}
