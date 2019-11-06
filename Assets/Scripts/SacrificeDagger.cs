using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeDagger : DivineWeapon
{
    protected override void OnWeaponPickup()
    {
        base.OnWeaponPickup();
        Debug.Log("Sacrifice Dagger Pickup");
    }

    public override void WeaponAttack(PlayerController player, Vector3 direction)
    {
        //base.WeaponAttack(player, direction);
        Debug.Log("Overriding Weapon Attack");

        player.Dash(direction);
        player.m_AttackHitboxController.ActivateHitBox(player.GetPlayerOrientation());

    }
}
