//TODO

//Structure fixes
//Make the attack hitbox route back to the weapon, not directly the player (AttackHitboxController)
//make class so player's control their health bars

//Pep Talk
//You're doing great my dude!

using UnityEngine;
public class HolyHammer : DivineWeapon
{
    [Header("Hammer Properties")]
    public GameObject m_KnockbackPrefab;
    public GameObject m_CrackPrefab;
    public float m_KnockbackRadius = 1.5f;
    public float m_BaseKnockbackForce = 200;
    public float m_DistanceForceStrength = 2.0f;
    //public float m_DistanceMaxMagnitude = 2.0f;
    public int m_HitStunTime = 12; //in frames

    [Header("Jump Properties")]
    public AnimationCurve m_JumpCurve;
    public float m_JumpHeight;
    public float m_LerpTightness = 2f;
    public float m_ForwardDistance = 0.5f;

    private PlayerController m_SlammingPlayer;
    private CameraShake m_CameraShake;
    private System.Action<PlayerController> m_OnKnockbackHit; //global so we can set it to null
    private bool m_IsSlamming;

    //jumping properties
    private Vector3 m_JumpDirection;
    private Vector3 m_PlayerStartingPos;

    private float m_JumpStartTime;
    private bool m_IsJumping;

    protected override void OverrideStart()
    {
        base.OverrideStart();
        m_CameraShake = FindObjectOfType<CameraShake>();
    }

    public override void OnWeaponPickup(PlayerController player)
    {
        base.OnWeaponPickup(player);
    }

    public override void OnWeaponAttackStart()
    {
        base.OnWeaponAttackStart();

        //set jump settings
        m_JumpDirection = m_PlayerRef.GetComponent<Rigidbody>().velocity.normalized;
        m_JumpStartTime = Time.time;
        m_PlayerStartingPos = m_PlayerRef.transform.position;
        m_IsJumping = true;
    }

    public override void WeaponAttack(PlayerController player, Vector3 direction)
    {
        base.WeaponAttack(player, direction);

        m_IsJumping = false; //jump only lasts for windup
        Vector3 playerStartingPos = new Vector3(m_PlayerRef.transform.position.x, m_PlayerStartingPos.y, m_PlayerRef.transform.position.z);
        m_PlayerRef.transform.position = playerStartingPos;

        StartSlam(player, direction);
    }

    public override void OnWeaponAttackEnd()
    {
        base.OnWeaponAttackEnd();
        m_IsSlamming = false;
    }

    private void StartSlam(PlayerController player, Vector3 direction)
    {
        if (m_IsSlamming) { return; }
        m_IsSlamming = true;

        //instantiate shockwave object at player location
        KnockbackSphere sphere = Instantiate(m_KnockbackPrefab).GetComponent<KnockbackSphere>();
        sphere.transform.position = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
        sphere.CreateSphere(3.0f, 10f/60f, player.gameObject.GetComponent<Collider>());
        sphere.m_OnSphereHit += OnSlamHit;

        //lock player movement
        player.ExternalDisablePlayerMovement(m_AttackAction.ActionLength / 60f, true); //convert from frames to seconds

        //add effects
        Vector3 effectsLocation = new Vector3(player.transform.position.x, m_ArenaCenter.position.y, player.transform.position.z);
        m_EffectsManager.ActivateEffect("Ground_Crack", effectsLocation);
        m_EffectsManager.ActivateEffect("Weapon_Sparks", effectsLocation);

        //hit stage
        Ray ray = new Ray(player.transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5.0f, ~LayerMask.NameToLayer("IgnoreFloor"))) //ignore everything but "ignorefloor"
        {
            CollapseableGround ground = hit.transform.gameObject.GetComponent<CollapseableGround>();
            if (ground != null)
            {
                ground.HitGround();
            }
        }

        //shake the camera
        m_CameraShake.AddTrauma(0.98f, .97f);
    }

    private void OnSlamHit(PlayerController hitPlayer, Vector3 origin)
    {
        Vector3 distVector = hitPlayer.transform.position - origin;
        float distMagnitude = distVector.sqrMagnitude;
        float force = (m_DistanceForceStrength / Mathf.Clamp(distMagnitude, 0.05f, 3.0f)) + m_BaseKnockbackForce;
        //float force = (1 / Mathf.Clamp(distMagnitude, 0.05f, 3.0f)) * m_DistanceMaxMagnitude + m_BaseKnockbackForce;

        Vector3 forceDirection = distVector.normalized;

        bool hitSuccessful = OnHit(hitPlayer, m_PlayerRef);

        if (hitSuccessful)
        {
            hitPlayer.ApplyKnockbackForce(forceDirection, force, ((float)m_HitStunTime / 60f));
            hitPlayer.GetComponent<JusticeUser>().SetAsHit((float)m_HitStunTime / 60f);
        }
        else
        {
            //knock back player who is about to get disarmed
            //by half as much force
            m_PlayerRef.ApplyKnockbackForce(forceDirection, force * 0.5f, ((float)m_HitStunTime / 60f));
        }

    }

    //deal damage and check for disarming
    public override bool OnHit(PlayerController hitPlayer, PlayerController attackingPlayer)
    {
        bool playerHit = base.OnHit(hitPlayer, attackingPlayer);

        if (playerHit)
        {

        }

        return playerHit;
    }

    protected override void Update()
    {
        base.Update();

        //Make player do a little hop
        if (m_IsJumping)
        {
            float endTime = m_JumpStartTime + (m_AttackAction.StartDelay / 60f);
            float clampedTime = Mathf.Clamp(Time.time, 0, endTime);
            float time = DeanUtils.Map(Time.time, m_JumpStartTime, endTime, 0, 1f);

            float jumpHeight = m_PlayerStartingPos.y + (m_JumpCurve.Evaluate(time) * m_JumpHeight);

            Vector3 jumpDistance = Vector3.Lerp(Vector3.zero, m_JumpDirection * m_ForwardDistance, time);

            Vector3 lerpEndPos = m_PlayerStartingPos + new Vector3(jumpDistance.x, jumpHeight, jumpDistance.z);
            Vector3 newPosition = Vector3.Lerp(m_PlayerRef.transform.position, lerpEndPos, Time.deltaTime * m_LerpTightness);

            m_PlayerRef.transform.position = newPosition;
        }
    }
}
