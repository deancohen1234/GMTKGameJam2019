using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
    public SystemInput m_SystemInput;

    private GameInput m_GlobalInput; //input used for entire game

    public void InitializeInput(bool useArcadeControls)
    {
        //setup input for gamemode (arcade vs normal)
        GameInput input;

        if (useArcadeControls)
        {
            Joystick p1Stick, p2Stick;

            GetArcadeJoysticks(out p1Stick, out p2Stick);

            input.P1_Stick = p1Stick.stick;
            input.P1_ActionButton = m_SystemInput.Arcade_P1_ActionButton;
            input.P1_DashButton = m_SystemInput.Arcade_P1_DashButton;
            input.P1_ConfirmButton = m_SystemInput.Arcade_P1_ConfirmButton;

            input.P2_Stick = p2Stick.stick;
            input.P2_ActionButton = m_SystemInput.Arcade_P2_ActionButton;
            input.P2_DashButton = m_SystemInput.Arcade_P2_DashButton;
            input.P2_ConfirmButton = m_SystemInput.Arcade_P2_ConfirmButton;
        }
        else
        {
            var allGamepads = Gamepad.all;

            if (allGamepads.Count != 2)
            {
                Debug.LogError("No more than two gamepads can be plugged in at a time");
                return;
            }

            input.P1_Stick = allGamepads[0].leftStick;
            input.P1_ActionButton = m_SystemInput.Normal_P1_ActionButton;
            input.P1_DashButton = m_SystemInput.Normal_P1_DashButton;
            input.P1_ConfirmButton = m_SystemInput.Normal_P1_ConfirmButton;

            input.P2_Stick = allGamepads[1].leftStick;
            input.P2_ActionButton = m_SystemInput.Normal_P2_ActionButton;
            input.P2_DashButton = m_SystemInput.Normal_P2_DashButton;
            input.P2_ConfirmButton = m_SystemInput.Normal_P2_ConfirmButton;

        }

        SetGlobalInput(input);
    }

    private void GetArcadeJoysticks(out Joystick p1_Stick, out Joystick p2_Stick)
    {
        p1_Stick = null;
        p2_Stick = null;

        //fill joystick objects
        var joysticks = Joystick.all;
        Debug.Log(joysticks.Count);

        if (joysticks.Count > 0)
        {
            for (int i = 0; i < joysticks.Count; i++)
            {
                if (joysticks[i].GetType() == typeof(Joystick))
                {
                    Joystick joystick = (Joystick)joysticks[i];

                    Debug.Log(joystick.valueSizeInBytes);
                    if (joystick.valueSizeInBytes > 5)
                    {
                        //arcade controller is greater than 5 bytes (40 bits)
                        continue;
                    }

                    if (p1_Stick == null)
                    {
                        p1_Stick = joystick;
                        continue;
                    }

                    if (p1_Stick.Equals(joystick))
                    {
                        //we are referencing P1 Joystick and it already has been filled
                        continue;
                    }
                    else if (p2_Stick == null)
                    {
                        p2_Stick = joystick;
                    }
                }
            }
        }
    }

    private void SetGlobalInput(GameInput gameInput)
    {
        m_GlobalInput.P1_Stick = gameInput.P1_Stick;
        m_GlobalInput.P1_ActionButton = gameInput.P1_ActionButton;
        m_GlobalInput.P1_DashButton = gameInput.P1_DashButton;
        m_GlobalInput.P1_ConfirmButton = gameInput.P1_ConfirmButton;

        m_GlobalInput.P2_Stick = gameInput.P2_Stick;
        m_GlobalInput.P2_ActionButton = gameInput.P2_ActionButton;
        m_GlobalInput.P2_DashButton = gameInput.P2_DashButton;
        m_GlobalInput.P2_ConfirmButton = gameInput.P2_ConfirmButton;

    }

    ////////////////////INPUT GETTERS////////////////////////////
    public Vector2 GetStickValue(bool isPlayerOne)
    {
        StickControl stick = isPlayerOne ? m_GlobalInput.P1_Stick : m_GlobalInput.P2_Stick;
        return stick.ReadValue();
    }

    public bool IsActionButtonPressedDown(bool isPlayerOne)
    {
        string inputString = isPlayerOne ? m_GlobalInput.P1_ActionButton : m_GlobalInput.P2_ActionButton;
        bool isPressed = Input.GetButtonDown(inputString);
        return isPressed;
    }

    public bool IsDashButtonPressedDown(bool isPlayerOne)
    {
        string inputString = isPlayerOne ? m_GlobalInput.P1_DashButton : m_GlobalInput.P2_DashButton;
        bool isPressed = Input.GetButtonDown(inputString);
        return isPressed;
    }

    public bool IsConfirmButtonPressedDown(bool isPlayerOne)
    {
        string inputString = isPlayerOne ? m_GlobalInput.P1_ConfirmButton : m_GlobalInput.P2_ConfirmButton;
        bool isPressed = Input.GetButtonDown(inputString);
        return isPressed;
    }

}

public struct GameInput
{
    public StickControl P1_Stick;
    public string P1_ActionButton;
    public string P1_DashButton;
    public string P1_ConfirmButton;

    public StickControl P2_Stick;
    public string P2_ActionButton;
    public string P2_DashButton;
    public string P2_ConfirmButton;
}
