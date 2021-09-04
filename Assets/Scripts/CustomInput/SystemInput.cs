using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputSetting", menuName = "Input/Input Setting")]
public class SystemInput : ScriptableObject
{
    [Header("Player 1 Conventional Input")]
    public string Normal_P1_HorizonalAxis;
    public string Normal_P1_VerticalAxis;
    public string Normal_P1_ActionButton;
    public string Normal_P1_DashButton;
    public string Normal_P1_ConfirmButton;

    [Header("Player 2 Conventional Input")]
    public string Normal_P2_HorizonalAxis;
    public string Normal_P2_VerticalAxis;
    public string Normal_P2_ActionButton;
    public string Normal_P2_DashButton;
    public string Normal_P2_ConfirmButton;

    [Header ("Player 1 Arcade Input")]
    public string Arcade_P1_HorizonalAxis;
    public string Arcade_P1_VerticalAxis;
    public string Arcade_P1_ActionButton;
    public string Arcade_P1_DashButton;
    public string Arcade_P1_ConfirmButton;

    [Header("Player 2 Arcade Input")]
    public string Arcade_P2_HorizonalAxis;
    public string Arcade_P2_VerticalAxis;
    public string Arcade_P2_ActionButton;
    public string Arcade_P2_DashButton;
    public string Arcade_P2_ConfirmButton;
}
