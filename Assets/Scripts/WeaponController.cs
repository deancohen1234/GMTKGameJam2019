using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private bool m_HasWeapon;

    public bool GetWeaponControl()
    {
        return m_HasWeapon;
    }
}
