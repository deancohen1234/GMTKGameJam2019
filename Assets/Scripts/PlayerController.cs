using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerType {Player1, Player2}
public enum PlayerOrientation { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft}
public enum PlayerSound { Damaged, Disarm, Death, Attack }

public struct InputStrings
{
    public string Horizontal;
    public string Vertical;
    public string RTrigger;
    public string LTrigger;
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthComponent))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public PlayerType m_PlayerNum;

    [Header("Move Properties")]
    public float m_DefaultMoveSpeed = 2.1f;

    [Header("Dash Properties")]
    public float m_DashSpeed = 15.0f;
    public int m_DisabledMovementTime = 6; //in frames

    [Header("Attack Properties")]
    public float m_AttackDistance = 1.0f;
    public float m_AttackSphereRadius = 0.35f;
    public float m_KnockbackForce = 2500.0f;

    [Header("Death Properties")]
    public float m_DeathLength = 4.0f; //in seconds

    [Header("References")]
    public ParticleSystem m_RunSystem;
    public SpriteHandler m_SpriteHandler;
    public AttackHitboxController m_AttackHitboxController;

    private InputStrings m_InputStrings;
    private Collider m_MainCollider;
    private Rigidbody m_Rigidbody;
    private AudioSource m_AudioSource;
    private HealthComponent m_HealthComponent;
    private CameraShake m_CameraShake;
    private RippleEffect m_RippleEffect;
    private PlayerAnimation m_PlayerAnimation;
    private EffectsController m_EffectsController;
    private HeadLauncher m_HeadLauncher;

    [Header("Actions")]
    public DashAction m_DashAction;
    public PlayerAction m_DisarmAction;
    //public PlayerAction m_DefaultDisarmAction;

    [Header("Sound Effects")]
    public AudioClip m_Poof;
    public AudioClip m_NudgeDisarm;
    public AudioClip m_SwordSlash;
    public AudioClip[] m_DamageSounds;
    public AudioClip[] m_DisarmSounds;
    public AudioClip[] m_DeathSounds;

    public float m_PitchMin = 0.7f;
    public float m_PitchMax = 1.3f;

    public GameObject m_WeaponIcon;
    public Action m_OnDeathComplete;

    private PlayerAction m_AttackAction;
    private DivineWeapon m_EquippedWeapon;
    private bool m_IsDisarming;

    private PlayerOrientation m_PlayerOrientation;

    private Vector3 lastMoveDirection;

    private float m_MoveSpeed; //move speed that can be default or weapon effected;
    private bool m_IsInvincible = false;
    private bool m_CanMove = true;
    private bool m_BlockAllInput = false;
    private bool m_ControllerDisabled = false; //enables controller to be disabled without killing whole player controller

    #region Monobehavior
    private void Awake()
    {
        m_MainCollider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        m_HealthComponent = GetComponent<HealthComponent>();
        m_PlayerAnimation = GetComponent<PlayerAnimation>();
        m_EffectsController = GetComponent<EffectsController>();
        m_CameraShake = FindObjectOfType<CameraShake>();
        m_RippleEffect = FindObjectOfType<RippleEffect>();
        m_HeadLauncher = GetComponent<HeadLauncher>();

        m_DashAction.SetPlayerReference(this);
        m_DisarmAction.SetPlayerReference(this);

        m_MoveSpeed = m_DefaultMoveSpeed;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupInputStrings(m_PlayerNum);

        m_AttackAction = m_DisarmAction;
        m_DashAction.IsAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        //TESTING
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_HealthComponent.Kill();
        }

        if (m_ControllerDisabled || m_BlockAllInput || m_HealthComponent.IsDead()) { return; }

        Vector2 input = Vector2.zero;
        bool LTriggerPressed = false;
        bool RTriggerPressed = false;

        if (m_PlayerNum == PlayerType.Player1)
        {
            input = ApplicationSettings.m_Singleton.m_InputManager.GetStickValue(true);

            LTriggerPressed = ApplicationSettings.m_Singleton.m_InputManager.IsDashButtonPressedDown(true);
            RTriggerPressed = ApplicationSettings.m_Singleton.m_InputManager.IsActionButtonPressedDown(true);
        }
        else
        {
            input = ApplicationSettings.m_Singleton.m_InputManager.GetStickValue(false);

            LTriggerPressed = ApplicationSettings.m_Singleton.m_InputManager.IsDashButtonPressedDown(false);
            RTriggerPressed = ApplicationSettings.m_Singleton.m_InputManager.IsActionButtonPressedDown(false);
        }

        input.Normalize(); //normalize any potenially weird input data       

        float x = input.x;
        float y = input.y;

        if (m_CanMove)
        {
            Move(x, y);
        }

        //if dash has been started
        m_DashAction.CheckActionCompleteness(x, y);
        m_AttackAction.CheckActionCompleteness(x, y);

        
        if (LTriggerPressed && m_DashAction.IsAvailable)
        {
            m_DashAction.ExecuteAction();
        }

        if (RTriggerPressed && m_AttackAction.IsAvailable)
        {
            m_AttackAction.ExecuteAction();
        }

        m_PlayerOrientation = CalculateOrientation(new Vector2(x, y).normalized);

        if (new Vector2(x, y).sqrMagnitude >= .2f)
        {
            //m_SpriteHandler.SetSprite(m_PlayerOrientation);
            m_PlayerAnimation.SetOrientation(m_PlayerOrientation);
            m_PlayerAnimation.SetPlayerMoveStatus(false);
        }
        else
        {
            m_PlayerAnimation.SetPlayerMoveStatus(true);
        }

    }

    private void OnEnable()
    {
        m_DashAction.OnActionStart += OnDashStart;
        m_DashAction.OnActionEnd += OnDashEnd;
        m_DashAction.ActionHandler += Dash;
        m_DashAction.OnDashDisarmEnd += OnDashDisarmEnd; 

        m_DisarmAction.ActionHandler += Disarm;
        m_DisarmAction.OnActionStart += OnAttackStartDecider;
        m_DisarmAction.OnActionEnd += OnAttackEndDecider;

        m_HealthComponent.m_OnDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        m_DashAction.OnActionStart -= OnDashStart;
        m_DashAction.OnActionEnd -= OnDashEnd;
        m_DashAction.ActionHandler -= Dash;
        m_DashAction.OnDashDisarmEnd -= OnDashDisarmEnd;

        m_DisarmAction.ActionHandler -= Disarm;
        m_DisarmAction.OnActionStart -= OnAttackStartDecider;
        m_DisarmAction.OnActionEnd -= OnAttackEndDecider;

        m_HealthComponent.m_OnDeath -= OnPlayerDeath;
    }
    #endregion

    #region Input
    private void SetupInputStrings(PlayerType playerNum)
    {
        switch (playerNum)
        {
            case PlayerType.Player1:
                m_InputStrings.Horizontal = "P1_Horizontal";
                m_InputStrings.Vertical = "P1_Vertical";
                m_InputStrings.RTrigger = "P1_RTrigger";
                m_InputStrings.LTrigger = "P1_LTrigger";
                break;
            case PlayerType.Player2:
                m_InputStrings.Horizontal = "P2_Horizontal";
                m_InputStrings.Vertical = "P2_Vertical";
                m_InputStrings.RTrigger = "P2_RTrigger";
                m_InputStrings.LTrigger = "P2_LTrigger";
                break;
        }
    }

    private PlayerOrientation CalculateOrientation(Vector2 vector)
    {
        PlayerOrientation orientation = PlayerOrientation.Up;
        float atan2 = Mathf.Atan2(vector.y, vector.x);

        atan2 = atan2 * Mathf.Rad2Deg;

        if (IsInRange(atan2, 67.5f, 112.5f))
        {
            //12 o clock
            orientation = PlayerOrientation.Up;
        }
        else if (IsInRange(atan2, 22.5f, 67.5f))
        {
            //1:30
            orientation = PlayerOrientation.UpRight;
        }
        else if (IsInRange(atan2, -22.5f, 22.5f))
        {
            //3:00
            orientation = PlayerOrientation.Right;
        }
        else if (IsInRange(atan2, -67.5f, -22.5f))
        {
            //4:30
            orientation = PlayerOrientation.DownRight;
        }
        else if (IsInRange(atan2, -112.5f, -67.5f))
        {
            //6:00
            orientation = PlayerOrientation.Down;
        }
        else if (IsInRange(atan2, -157.5f, -112.5f))
        {
            orientation = PlayerOrientation.DownLeft;
        }
        else if (IsInRange(atan2, 112.5f, 157.5f))
        {
            //10:30
            orientation = PlayerOrientation.UpLeft;
        }
        else
        {
            //9
            orientation = PlayerOrientation.Left;
        }

        return orientation;
    }

    private void Move(float x, float y)
    {
        Vector3 inputVelocity = new Vector3(x, 0f, y) * m_MoveSpeed; //no delta time because physcis is doing that for us

        m_Rigidbody.velocity = inputVelocity;

        if (x != 0 || y != 0)
        {
            lastMoveDirection = m_Rigidbody.velocity.normalized;
        }
    }

    #endregion

    public void DisableController(bool stopAllVelocity = true)
    {
        m_ControllerDisabled = true;

        if (stopAllVelocity)
        {
            m_Rigidbody.velocity = Vector3.zero;
        }
    }

    #region Weapon Methods

    public void EquipWeapon(DivineWeapon weapon)
    {
        //only equip weapon when you have none in hand
        if (HasWeapon() == false)
        {
            m_EquippedWeapon = weapon;
            m_WeaponIcon.SetActive(true);

            m_EquippedWeapon.OnPickup(this);
            m_AttackAction = m_EquippedWeapon.m_AttackAction;

            //set player move speed to weapon move speed
            m_MoveSpeed = m_MoveSpeed * m_EquippedWeapon.m_PlayerSpeedModifier;
        }
    }

    //if true, then weapon is gone completlely, not dropped (ie being disarmed)
    public void DropWeapon(bool loseCompletely)
    {
        if (m_EquippedWeapon != null)
        {
            m_AttackAction.ForceStopAction();

            if (!loseCompletely)
            {
                m_EquippedWeapon.Drop(this);
            }

            m_EquippedWeapon = null;
            m_WeaponIcon.SetActive(false);

            m_AttackAction = m_DisarmAction; //when losing weapon, attack action is now disarming

            m_AttackAction.IsExecuting = false;

            //when losing weapon, set move speed back to default
            m_MoveSpeed = m_DefaultMoveSpeed;

        }

    }
    #endregion

    #region Audio
    public void PlaySoundEffect(PlayerSound playerSound)
    {
        switch (playerSound)
        {
            case PlayerSound.Damaged:
                PlaySoundFromArray(m_DamageSounds);
                break;
            case PlayerSound.Disarm:
                PlaySoundFromArray(m_DisarmSounds);
                break;
            case PlayerSound.Death:
                PlaySoundFromArray(m_DeathSounds);
                break;
            case PlayerSound.Attack:
                //TODO make this an array
                PlaySound(m_SwordSlash);
                break;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        m_AudioSource.pitch = UnityEngine.Random.Range(m_PitchMin, m_PitchMax);

        m_AudioSource.clip = clip;
        m_AudioSource.Play();
    }

    public void PlayPoofSound()
    {
        m_AudioSource.clip = m_Poof;
        m_AudioSource.Play();
    }

    private void PlaySoundFromArray(AudioClip[] allSoundClips)
    {
        int randomClipIndex = UnityEngine.Random.Range(0, allSoundClips.Length - 1);
        AudioClip clip = allSoundClips[randomClipIndex];

        m_AudioSource.pitch = UnityEngine.Random.Range(m_PitchMin, m_PitchMax);

        m_AudioSource.clip = clip;
        m_AudioSource.Play();
    }
    #endregion

    #region Action Methods
    //when player is attacking this object
    public void AttemptMeleeAttack(PlayerController attackingPlayer)
    {
        m_CameraShake.AddTrauma(0.5f);

        if (attackingPlayer.HasWeapon())
        {
            if (!attackingPlayer.m_DashAction.IsExecuting)
            {
                AttackHit(attackingPlayer.m_EquippedWeapon, attackingPlayer);
            }
        }
        else
        {
            DashHit(attackingPlayer);
        }
    }

    public void AttackHit(IWeapon attackingWeapon, PlayerController attackingPlayer)
    {
        if (attackingPlayer != null)
        {
            attackingWeapon.OnHit(this, attackingPlayer);
        }
        else
        {
            Debug.LogError("Attack hit has null attacking");
        }
    }

    private void DashHit(PlayerController attackingPlayer)
    {
        attackingPlayer.m_DashAction.ForceStopAction();

        //attacking player loses weapon, no damage
        attackingPlayer.ApplyBounceBackForce(transform.position);
        ApplyBounceBackForce(attackingPlayer.gameObject.transform.position);

        //attacking player hit this player, lose your weapon
        if (HasWeapon())
        {
            m_AudioSource.clip = m_NudgeDisarm;
            m_AudioSource.Play();

            DropWeapon(false);
            m_CanMove = true;

        }
    }

    public void DisarmOppponent(DivineWeapon weapon, PlayerController attackingPlayer)
    {
        m_RippleEffect.ActivateRipple(transform.position);
        m_AttackAction.ForceStopAction();

        weapon.SetWeaponActive(false);
        EquipWeapon(weapon);

        m_CanMove = true;

        //if there is an attacking disarm them
        if (attackingPlayer)
        {
            attackingPlayer.m_AttackAction.ForceStopAction();

            //store reference to weapon, then have attacking player drop the weapon and have disarming player equip it
            attackingPlayer.DropWeapon(true);

            attackingPlayer.m_CanMove = true;
        }


        m_EffectsController.ActivateOnDisarmedSystem(transform.position);

        int randomClipIndex = UnityEngine.Random.Range(0, m_DamageSounds.Length - 1);
        AudioClip clip = m_DisarmSounds[randomClipIndex];

        m_AudioSource.clip = clip;
        m_AudioSource.Play();
    }

    private void OnAttackStartDecider()
    {
        if (m_EquippedWeapon != null)
        {
            m_PlayerAnimation.SetAttackStatus(true);
            m_EffectsController.StartVisualAttack();

            m_RunSystem.Emit(15);
            m_RunSystem.Play();

            PlaySound(m_EquippedWeapon.m_AttackSound);
        }
        else
        {
            OnDisarmStart();
        }
    }

    private void OnAttackEndDecider()
    {
        if (m_EquippedWeapon != null)
        {
            m_AttackHitboxController.DisableAllHitBoxes();
            m_PlayerAnimation.SetAttackStatus(false);

            m_EffectsController.EndVisualAttack();
        }
        else
        {
            OnDisarmEnd();
        }

        m_RunSystem.Stop(); //not in an if statement just incase player got knocked out of the attack
    }

    private void OnDefaultAttackStart()
    {
        m_PlayerAnimation.SetAttackStatus(true);

        m_EffectsController.StartVisualAttack();
    }

    private void OnAttackEnd()
    {
        m_AttackHitboxController.DisableAllHitBoxes();
        m_PlayerAnimation.SetAttackStatus(false);

        m_EffectsController.EndVisualAttack();
    }

    #endregion

    public void SetWeaponIcon(Sprite sprite)
    {
        m_WeaponIcon.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void SetIsInvincible (bool b)
    {
        m_IsInvincible = b;
    }

    public void ApplyBounceBackForce(Vector3 forceOrigin)
    {
        Debug.Log("Bounce Back Force applied");

        StartCoroutine(DisableAllMovement(0.2f));

        Vector3 direction = transform.position - forceOrigin;
        m_Rigidbody.AddForce(direction.normalized * m_KnockbackForce);

        /*m_Rigidbody.velocity = -direction.normalized * m_KnockbackForce;
        otherPlayer.m_Rigidbody.velocity = direction.normalized * m_KnockbackForce;*/
    }

    public void ApplyKnockbackForce(Vector3 direction, float force, float hitStunTime)
    {
        m_Rigidbody.velocity = Vector3.zero;

        StartCoroutine(DisableAllMovement(hitStunTime));

        m_Rigidbody.AddForce(direction.normalized * force);
    }



    public void ExternalDisablePlayerMovement(float time, bool freezePlayerPosition = false)
    {
        if (freezePlayerPosition)
        {
            m_Rigidbody.velocity = Vector3.zero;
        }

        StartCoroutine(DisablePlayerMovement(time));
    }

    

    private IEnumerator DisablePlayerMovement(float time)
    {
        m_CanMove = false;

        yield return new WaitForSeconds(time);

        m_CanMove = true;
    }

    private IEnumerator DisableAllMovement(float time)
    {
        m_BlockAllInput = true;

        yield return new WaitForSeconds(time);

        m_BlockAllInput = false;
    }

    #region Dashing
    private void OnDashStart()
    {
        m_PlayerAnimation.SetDashStatus(true);
        //GetComponent<Renderer>().material.color = Random.ColorHSV();
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV();

        m_RunSystem.Emit(15);
        m_RunSystem.Play();
    }

    public void Dash(PlayerController player, Vector3 direction)
    {
        StartCoroutine(DisablePlayerMovement((float)m_DisabledMovementTime / 60f));

        m_Rigidbody.velocity = direction * m_DashSpeed;

        m_AttackHitboxController.ActivateHitBox(m_PlayerOrientation); //for nudging weapon out of player's hand
    }

    private void OnDashEnd()
    {
        m_PlayerAnimation.SetDashStatus(false);
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.white;
        m_RunSystem.Stop();
    }

    private void OnDashDisarmEnd()
    {
        m_AttackHitboxController.DisableAllHitBoxes();
        Debug.Log("Ending hitboxes");
    }

    #endregion

    #region Disarming
    private void OnDisarmStart()
    {
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.black;
        m_PlayerAnimation.StartDisarm();
        m_EffectsController.ActivateDisarmSystem();
    }

    private void Disarm(PlayerController player, Vector3 direction)
    {
        m_IsDisarming = true;

        m_Rigidbody.velocity = Vector3.zero;
        StartCoroutine(DisablePlayerMovement((float)m_DashAction.ActionLength / 60f));
    }

    private void OnDisarmEnd()
    {
        m_IsDisarming = false;
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.white;
        m_EffectsController.DeActiviateDisarmSystem();

    }
    #endregion

    #region Death
    private void OnPlayerDeath()
    {
        //gameObject.SetActive(false);
        m_EffectsController.m_CharacterSprite.enabled = false; //hide player sprite
        m_EffectsController.ActivateOnDeathSystem();

        m_HeadLauncher.LaunchHead(transform.position, m_CameraShake.gameObject.transform.position);

        int randomClipIndex = UnityEngine.Random.Range(0, m_DeathSounds.Length - 1);
        AudioClip clip = m_DeathSounds[randomClipIndex];
        m_AudioSource.clip = clip;
        m_AudioSource.pitch = UnityEngine.Random.Range(m_PitchMin, m_PitchMax);
        m_AudioSource.Play();

        Invoke("CompleteDeath", m_DeathLength);
    }

    private void CompleteDeath()
    {
        m_OnDeathComplete?.Invoke();
    }
    #endregion

    #region Getter Methods
    public PlayerOrientation GetPlayerOrientation()
    {
        return m_PlayerOrientation;
    }

    public HealthComponent GetHealthComponent()
    {
        return m_HealthComponent;
    }

    public EffectsController GetEffectsController()
    {
        return m_EffectsController;
    }

    public bool HasWeapon()
    {
        bool hasWeapon = (m_EquippedWeapon != null) ? true : false;
        return hasWeapon;
    }

    public bool IsDisarming()
    {
        return m_IsDisarming;
    }

    public bool IsStunned()
    {
        return m_BlockAllInput;
    }

    public bool IsInvincible()
    {
        return m_IsInvincible;
    }

    public Vector3 GetMoveDirection()
    {
        if (m_Rigidbody != null)
        {
            if (m_Rigidbody.velocity == Vector3.zero)
            {
                return lastMoveDirection;
            }
            else
            {
                return m_Rigidbody.velocity.normalized;
            }
        }

        return Vector3.zero;
    }

    public Collider GetMainCollider()
    {
        return m_MainCollider;
    }

    //inclusive min and exclusive max
    private bool IsInRange(float value, float min, float max)
    {
        if (value >= min && value < max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}

