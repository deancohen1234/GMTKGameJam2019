using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System;

public enum PlayerType {Player1, Player2}
public enum PlayerOrientation { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft}
public enum PlayerSound { Damaged, Disarm, Death }

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
    public float m_MoveSpeed = 5.0f;

    [Header("Dash Properties")]
    public float m_DashSpeed = 15.0f;
    public int m_DisabledMovementTime = 6; //in frames

    [Header("Attack Properties")]
    public float m_AttackDistance = 1.0f;
    public float m_AttackSphereRadius = 0.35f;
    public float m_KnockbackForce = 2500.0f;

    [Header("Death Properties")]
    public float m_DeathLength = 4.0f; //in seconds

    public SpriteHandler m_SpriteHandler;
    public AttackHitboxController m_AttackHitboxController;

    private InputStrings m_InputStrings;
    private Rigidbody m_Rigidbody;
    private AudioSource m_AudioSource;
    private HealthComponent m_HealthComponent;
    private CameraShake m_CameraShake;
    private RippleEffect m_RippleEffect;
    private PlayerAnimation m_PlayerAnimation;
    private EffectsController m_EffectsController;
    private HeadLauncher m_HeadLauncher;

    [Header("Actions")]
    public PlayerAction m_DashAction;
    public PlayerAction m_AttackAction;
    //public PlayerAction m_DefaultDisarmAction;

    [Header("Sound Effects")]
    public AudioClip m_Poof;
    public AudioClip m_NudgeDisarm;
    public AudioClip m_SwordSlash;
    public AudioClip[] m_DamageSounds;
    public AudioClip[] m_DisarmSounds;
    public AudioClip[] m_DeathSounds;

    public GameObject m_WeaponIcon;
    public Action m_OnDeathComplete;

    private DivineWeapon m_EquippedWeapon;
    private bool m_IsDisarming;

    private PlayerOrientation m_PlayerOrientation;
    private bool m_CanMove = true;
    private bool m_BlockAllInput = false;
    private Vector3 m_AttackDirection; //need to store attack direction for dash attacking

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        m_HealthComponent = GetComponent<HealthComponent>();
        m_PlayerAnimation = GetComponent<PlayerAnimation>();
        m_EffectsController = GetComponent<EffectsController>();
        m_CameraShake = FindObjectOfType<CameraShake>();
        m_RippleEffect = FindObjectOfType<RippleEffect>();
        m_HeadLauncher = GetComponent<HeadLauncher>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupInputStrings(m_PlayerNum);

        m_DashAction.IsAvailable = true;
    }

    //
    private void OnEnable()
    {
        m_DashAction.OnActionStart += OnDashStart;
        m_DashAction.OnActionEnd += OnDashEnd;
        m_DashAction.ActionHandler += Dash;

        m_AttackAction.ActionHandler += AttackDecider;
        m_AttackAction.OnActionStart += OnAttackStartDecider;
        m_AttackAction.OnActionEnd += OnAttackEndDecider;

        m_HealthComponent.m_OnDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        m_DashAction.OnActionStart -= OnDashStart;
        m_DashAction.OnActionEnd -= OnDashEnd;
        m_DashAction.ActionHandler -= Dash;

        m_AttackAction.ActionHandler -= AttackDecider;
        m_AttackAction.OnActionStart -= OnAttackStartDecider;
        m_AttackAction.OnActionEnd -= OnAttackEndDecider;

        m_HealthComponent.m_OnDeath -= OnPlayerDeath;
    }

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

    // Update is called once per frame
    void Update()
    {
        if (m_BlockAllInput || m_HealthComponent.IsDead()) { return; }

        var gamepads = Gamepad.all;
        Vector2 input = Vector2.zero;

        if (gamepads.Count < 2)
        {
            Debug.LogError("Less than two inputs found");
            //return;
        }

        if (m_PlayerNum == PlayerType.Player1)
        {
            input = gamepads[0].leftStick.ReadValue();
        }
        else
        {
            input = gamepads[1].leftStick.ReadValue();
        }

        float x = input.x;
        float y = input.y;

        if (m_CanMove)
        {
            Move(x, y);
        }

        //if dash has been started
        m_DashAction.CheckActionCompleteness(x, y);
        m_AttackAction.CheckActionCompleteness(x, y);

        bool LTriggerPressed = false;
        bool RTriggerPressed = false;
        if (m_PlayerNum == PlayerType.Player1)
        {
            LTriggerPressed = gamepads[0].leftTrigger.isPressed;
            RTriggerPressed = gamepads[0].rightTrigger.isPressed;
        }
        else
        {
            LTriggerPressed = gamepads[1].leftTrigger.isPressed;
            RTriggerPressed = gamepads[1].rightTrigger.isPressed;
        }

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
    }

    public void DisableAllMovement()
    {
        m_BlockAllInput = true;
        m_Rigidbody.velocity = Vector3.zero;
    }

    public void PlayPoofEffect()
    {
        m_AudioSource.clip = m_Poof;
        m_AudioSource.Play();
    }

    public void EquipWeapon(DivineWeapon weapon)
    {
        //only equip weapon when you have none in hand
        if (HasWeapon() == false)
        {
            m_EquippedWeapon = weapon;
            m_WeaponIcon.SetActive(true);
        }
    }

    public void DropWeapon()
    {
        if (m_EquippedWeapon != null)
        {
            m_EquippedWeapon.Drop(this);

            m_EquippedWeapon = null;
            m_WeaponIcon.SetActive(false);

            m_AttackAction.IsExecuting = false;
        }

    }

    //have weapon completely be lost
    public void LoseWeapon()
    {
        if (m_EquippedWeapon != null)
        {
            m_EquippedWeapon = null;
            m_WeaponIcon.SetActive(false);

            m_AttackAction.IsExecuting = false;
        }

    }

    public void PlaySoundEffect(PlayerSound playerSound)
    {
        switch (playerSound)
        {
            case PlayerSound.Damaged:
                PlayRandomSound(m_DamageSounds);
                break;
            case PlayerSound.Disarm:
                PlayRandomSound(m_DisarmSounds);
                break;
            case PlayerSound.Death:
                PlayRandomSound(m_DeathSounds);
                break;
        }
    }

    private void PlayRandomSound(AudioClip[] allSoundClips)
    {
        int randomClipIndex = UnityEngine.Random.Range(0, allSoundClips.Length - 1);
        AudioClip clip = allSoundClips[randomClipIndex];

        m_AudioSource.clip = clip;
        m_AudioSource.Play();
    }

    public void ApplyBounceBackForce(Vector3 otherPlayerPos)
    {
        StartCoroutine(DisableAllMovement(0.2f));

        Vector3 direction = transform.position - otherPlayerPos;
        m_Rigidbody.AddForce(direction.normalized * m_KnockbackForce);

        /*m_Rigidbody.velocity = -direction.normalized * m_KnockbackForce;
        otherPlayer.m_Rigidbody.velocity = direction.normalized * m_KnockbackForce;*/

    }

    #region Action Methods

    //when player is attacking this object
    public void AttemptAttack(PlayerController attackingPlayer)
    {
        m_CameraShake.AddTrauma(0.5f);

        if (attackingPlayer.HasWeapon())
        {
            if (!attackingPlayer.m_DashAction.IsExecuting)
            {
                AttemptAttackHit(attackingPlayer);
            }
        }
        else
        {
            AttemptDashHit(attackingPlayer);
        }
    }

    private void AttemptAttackHit(PlayerController attackingPlayer)
    {
        attackingPlayer.m_EquippedWeapon.OnHit(this, attackingPlayer);
        
    }

    private void AttemptDashHit(PlayerController attackingPlayer)
    {
        Debug.Log("Has Weapon, About to Lose it");

        //attacking player loses weapon, no damage
        attackingPlayer.ApplyBounceBackForce(transform.position);
        ApplyBounceBackForce(attackingPlayer.gameObject.transform.position);

        //attacking player hit this player, lose your weapon
        if (HasWeapon())
        {
            m_AudioSource.clip = m_NudgeDisarm;
            m_AudioSource.Play();
            //m_EquippedWeapon.RandomizeLocation();
            DropWeapon();
        }
    }

    public void DisarmOppponent(PlayerController attackingPlayer)
    {
        m_RippleEffect.ActivateRipple(transform.position);

        m_AttackAction.ForceStopAction();
        attackingPlayer.m_AttackAction.ForceStopAction();

        EquipWeapon(attackingPlayer.m_EquippedWeapon);
        attackingPlayer.LoseWeapon();

        attackingPlayer.m_CanMove = true;
        m_CanMove = true;

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
        }
        else
        {
            OnDisarmStart();
        }
    }

    //press right trigger and do action based on whether or not you have weapon
    private void AttackDecider(Vector3 direction)
    {
        Debug.Log("Attacking...");
        if (m_EquippedWeapon != null)
        {
            m_EquippedWeapon.WeaponAttack(this, direction);
        }
        else
        {
            Disarm(direction);
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
    }

    private void OnDefaultAttackStart()
    {
        m_PlayerAnimation.SetAttackStatus(true);

        m_EffectsController.StartVisualAttack();
    }

    private void Attack(Vector3 direction)
    {
        Dash(direction);

        m_AudioSource.clip = m_SwordSlash;
        m_AudioSource.pitch = UnityEngine.Random.Range(.7f, 1.3f);
        m_AudioSource.Play();

        m_AttackHitboxController.ActivateHitBox(m_PlayerOrientation);
    }

    private void OnAttackEnd()
    {
        m_AttackHitboxController.DisableAllHitBoxes();
        m_PlayerAnimation.SetAttackStatus(false);

        m_EffectsController.EndVisualAttack();
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

    private void OnDashStart()
    {
        m_PlayerAnimation.SetDashStatus(true);
        //GetComponent<Renderer>().material.color = Random.ColorHSV();
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV();
    }

    public void Dash(Vector3 direction)
    {
        StartCoroutine(DisablePlayerMovement((float)m_DisabledMovementTime / 60f));

        m_Rigidbody.velocity = direction * m_DashSpeed;

        m_AttackHitboxController.ActivateHitBox(m_PlayerOrientation); //for nudging weapon out of player's hand
    }

    private void OnDashEnd()
    {
        m_PlayerAnimation.SetDashStatus(false);
        m_AttackHitboxController.DisableAllHitBoxes();
        //GetComponent<Renderer>().material.color = Color.white;
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnDisarmStart()
    {
        m_SpriteHandler.GetComponent<SpriteRenderer>().color = Color.black;
        m_PlayerAnimation.StartDisarm();
        m_EffectsController.ActivateDisarmSystem();
    }

    private void Disarm(Vector3 direction)
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

    //player dies
    private void OnPlayerDeath()
    {
        //gameObject.SetActive(false);
        m_EffectsController.m_CharacterSprite.enabled = false; //hide player sprite
        m_EffectsController.ActivateOnDeathSystem();
        m_HeadLauncher.LaunchHead(transform.position, m_CameraShake.gameObject.transform.position);

        int randomClipIndex = UnityEngine.Random.Range(0, m_DeathSounds.Length - 1);
        AudioClip clip = m_DeathSounds[randomClipIndex];
        m_AudioSource.clip = clip;
        m_AudioSource.pitch = UnityEngine.Random.Range(.7f, 1.3f);
        m_AudioSource.Play();

        Invoke("CompleteDeath", m_DeathLength);
    }

    private void CompleteDeath()
    {
        m_OnDeathComplete.Invoke();
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
    #endregion
}

[System.Serializable]
public class PlayerAction
{
    [HideInInspector]
    public bool IsAvailable = true;
    [HideInInspector]
    public bool IsExecuting = false;

    public int CooldownTime; //in frames
    public int StartDelay; //in frames
    public int ActionLength; //in frames

    public Action OnActionStart;
    public Action OnActionEnd;
    public Action<Vector3> ActionHandler; //holds references to other scripts that actually do action like dash and attack

    private int StartingFrame;

    public void SetStartingFrame(int frameNum)
    {
        StartingFrame = frameNum;
    }

    public int GetStartingFrame()
    {
        return StartingFrame;
    }

    public void ExecuteAction()
    {
        StartingFrame = Time.frameCount;
        IsAvailable = false;
        IsExecuting = true;

        if (OnActionStart != null)
        {
            OnActionStart.Invoke();
        }
    }

    public void ForceStopAction()
    {
        IsExecuting = false;

        OnActionEnd.Invoke();
    }

    //called every frame once action starts
    public void CheckActionCompleteness(float xInput, float yInput)
    {
        //if dash has been started
        if (IsExecuting)
        {
            if (Time.frameCount - StartingFrame >= ActionLength + StartDelay)
            {
                //End dash
                StartingFrame = Time.frameCount; //star timer for cooldown
                IsExecuting = false;

                if (OnActionEnd != null)
                {
                    OnActionEnd.Invoke();
                }
            }
            //start delay finished
            else if (Time.frameCount - StartingFrame >= StartDelay)
            {
                //Do dash
                Vector3 direction = new Vector3(xInput, 0, yInput);

                ActionHandler.DynamicInvoke(direction);
            }

        }

        //if action is complete and cooldown still is going on
        else if (!IsAvailable)
        {
            //check cooldown
            if (Time.frameCount - StartingFrame >= CooldownTime)
            {
                StartingFrame = 0;
                IsAvailable = true;
            }
        }
    }
}
