using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyHammer : DivineWeapon
{
    [Header("Hammer Properties")]
    public AnimationCurve m_JumpPath;
    public float m_JumpRange = 1.3f;
    public float m_JumpModifier = 1.5f;

    private PlayerController m_SlammingPlayer;
    private Vector3 m_StartingLoc;
    private Vector3 m_DestinationLoc;
    private float m_ActionLength;
    private float m_StartTime;
    private bool m_IsSlamming;

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
        m_PlayerRef.m_AttackHitboxController.DisableAllHitBoxes();
    }

    private void StartSlam(PlayerController player, Vector3 direction)
    {
        if (m_IsSlamming) { return; }

        Debug.Log("Starting Slam");
        Vector3 destinationLoc = player.transform.position + (direction * m_JumpRange);
        destinationLoc.y = player.transform.position.y;

        m_DestinationLoc = destinationLoc;
        m_SlammingPlayer = player;
        m_ActionLength = m_AttackAction.ActionLength / 60f; //convert from frames to seconds
        m_StartingLoc = player.transform.position;
        m_StartTime = Time.time;

        player.ExternalDisablePlayerMovement(m_ActionLength);
        player.m_AttackHitboxController.ActivateHitBox(player.GetPlayerOrientation());

        m_IsSlamming = true;
    }

    protected override void Update()
    {
        base.Update();

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
        }
    }
}
