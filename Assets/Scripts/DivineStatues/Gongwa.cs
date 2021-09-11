using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gongwa : DivineStatue
{
    private DivineWeapon m_Weapon;

    public override void OnGameComplete()
    {

    }

    public override void OnGameIntialized()
    {

    }

    public override void OnRoundComplete()
    {

    }

    public override void OnRoundStarted()
    {
        Invoke("DelayedStart", 2.0f);
    }

    public override void SetDivineWeapon(DivineWeapon weapon)
    {
        m_Weapon = weapon;
    }

    public override void SetPlayers(PlayerController p1, PlayerController p2)
    {

    }

    private void DelayedStart()
    {
        m_Weapon.RandomizeLocationFromCeiling();
    }
}
