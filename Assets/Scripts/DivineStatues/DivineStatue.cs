using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DivineStatue : MonoBehaviour
{
    //called from the round manager
    public abstract void OnGameIntialized();
    public abstract void OnGameComplete();
    public abstract void SetDivineWeapon(DivineWeapon weapon);
    public abstract void SetPlayers(PlayerController p1, PlayerController p2);
    public abstract void OnRoundStarted();
    public abstract void OnRoundComplete();
}
