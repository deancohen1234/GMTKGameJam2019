using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeDagger : DivineWeapon
{
    public override void OnWeaponPickup(PlayerController player)
    {
        base.OnWeaponPickup(player);
    }

    public override void WeaponAttack(PlayerController player, Vector3 direction)
    {
        base.WeaponAttack(player, direction);

        player.Dash(player, direction);
        player.m_AttackHitboxController.ActivateHitBox(player.GetPlayerOrientation());
    }

    public override void OnWeaponAttackEnd()
    {
        base.OnWeaponAttackEnd();
        m_PlayerRef.m_AttackHitboxController.DisableAllHitBoxes();
    }
}
