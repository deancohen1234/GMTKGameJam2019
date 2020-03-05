//TODO
//Override OnHit to not deal damage and not apply bounceback
//make stage fall off able

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
    public float m_KnockbackRadius = 1.5f;
    public float m_BaseKnockbackForce = 200;
    public float m_DistanceForceStrength = 2.0f;
    public int m_HitStunTime = 12; //in frames

    public AnimationCurve m_JumpPath;
    public float m_JumpRange = 1.3f;
    public float m_JumpModifier = 1.5f;

    private PlayerController m_SlammingPlayer;
    private Vector3 m_StartingLoc;
    private Vector3 m_DestinationLoc;
    private float m_StartTime;
    private bool m_IsSlamming;
    private System.Action<PlayerController> m_OnKnockbackHit; //global so we can set it to null

    public override void OnWeaponPickup(PlayerController player)
    {
        base.OnWeaponPickup(player);
    }

    public override void OnWeaponAttackStart()
    {
        base.OnWeaponAttackStart();
    }

    public override void WeaponAttack(PlayerController player, Vector3 direction)
    {
        base.WeaponAttack(player, direction);
        StartSlam2(player, direction);
    }

    public override void OnWeaponAttackEnd()
    {
        base.OnWeaponAttackEnd();
        m_IsSlamming = false;
    }

    private void StartSlam(PlayerController player, Vector3 direction)
    {
        if (m_IsSlamming) { return; }

        Debug.Log("Starting Slam");
        Vector3 destinationLoc = player.transform.position + (direction * m_JumpRange);
        destinationLoc.y = player.transform.position.y;

        m_DestinationLoc = destinationLoc;
        m_SlammingPlayer = player;
        m_StartingLoc = player.transform.position;
        m_StartTime = Time.time;

        player.m_AttackHitboxController.ActivateHitBox(player.GetPlayerOrientation());

        m_IsSlamming = true;
    }

    private void StartSlam2(PlayerController player, Vector3 direction)
    {
        if (m_IsSlamming) { return; }
        m_IsSlamming = true;

        //instantiate shockwave object at player location
        KnockbackSphere sphere = Instantiate(m_KnockbackPrefab).GetComponent<KnockbackSphere>();
        sphere.transform.position = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
        sphere.CreateSphere(3.0f, 1.0f, player.gameObject.GetComponent<Collider>());
        sphere.m_OnSphereHit += OnSlamHit;

        //lock player movement
        player.ExternalDisablePlayerMovement(m_AttackAction.ActionLength / 60f); //convert from frames to seconds

    }

    private void OnSlamHit(PlayerController hitPlayer, Vector3 origin)
    {
        Vector3 distVector = hitPlayer.transform.position - origin;
        float distMagnitude = distVector.sqrMagnitude;
        float force = (m_DistanceForceStrength / Mathf.Clamp(distMagnitude, 0.05f, 3.0f)) + m_BaseKnockbackForce;

        Vector3 forceDirection = distVector.normalized;

        hitPlayer.ApplyKnockbackForce(forceDirection, force, ((float)m_HitStunTime / 60f));
        OnHit(hitPlayer, m_PlayerRef);
    }

    protected override void Update()
    {
        base.Update();
        /*
        if (m_IsSlamming)
        {
            float lerpTime = DeanUtils.Map(Time.time, m_StartTime, m_StartTime + m_ActionLength, 0.0f, 1.0f);
            Debug.Log("Lerp Time: " + lerpTime);

            if (lerpTime >= 1.0f)
            {
                m_PlayerRef.transform.position = m_DestinationLoc;
                m_IsSlamming = false;
                return;
            }
            Vector3 newPlayerPosition = Vector3.Lerp(m_StartingLoc, m_DestinationLoc, lerpTime);
            newPlayerPosition.y = m_JumpPath.Evaluate(lerpTime) * m_JumpModifier;

            m_PlayerRef.transform.position = newPlayerPosition;
        }*/
    }
}
