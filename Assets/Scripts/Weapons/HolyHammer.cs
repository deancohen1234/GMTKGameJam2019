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

    private PlayerController m_SlammingPlayer;
    private CameraShake m_CameraShake;
    private System.Action<PlayerController> m_OnKnockbackHit; //global so we can set it to null
    private bool m_IsSlamming;

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
    }

    public override void WeaponAttack(PlayerController player, Vector3 direction)
    {
        base.WeaponAttack(player, direction);
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
}
