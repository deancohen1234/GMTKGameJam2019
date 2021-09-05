
using UnityEngine;

public interface IWeapon
{
    public void OnPickup(PlayerController player);
    public void OnAttackStart();
    public void Attack(PlayerController player, Vector3 direction);
    public void OnAttackEnd();
    public void Drop(PlayerController lastControlledPlayer);
    public bool OnHit(PlayerController hitPlayer, PlayerController attackingPlayer);
    public void SetWeaponActive(bool isActive);
}
