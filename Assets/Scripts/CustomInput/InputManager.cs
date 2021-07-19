using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputManager : MonoBehaviour
{
    public SystemInput m_SystemInput;

    public float m_DeadZone = 0.15f;

    private GameInput m_GlobalInput; //input used for entire game
    private float m_LastInputTime;
    private bool m_BlockInput = false;

    public void InitializeInput(bool useArcadeControls)
    {
        //setup input for gamemode (arcade vs normal)
        GameInput input;

        if (useArcadeControls)
        {
            Joystick p1Stick, p2Stick;

            GetArcadeJoysticks(out p1Stick, out p2Stick);

            input.P1_Stick = p1Stick.stick;
            input.P1_ActionButton = new GameButton(null, m_SystemInput.Arcade_P1_ActionButton, false);
            input.P1_DashButton = new GameButton(null, m_SystemInput.Arcade_P1_DashButton, false);
            input.P1_ConfirmButton = new GameButton(null, m_SystemInput.Arcade_P1_ConfirmButton, false);

            input.P2_Stick = p2Stick.stick;
            input.P2_ActionButton = new GameButton(null, m_SystemInput.Arcade_P2_ActionButton, false);
            input.P2_DashButton = new GameButton(null, m_SystemInput.Arcade_P2_DashButton, false);
            input.P2_ConfirmButton = new GameButton(null, m_SystemInput.Arcade_P2_ConfirmButton, false);
        }
        else
        {
            var allGamepads = Gamepad.all;

            if (allGamepads.Count != 2)
            {
                Debug.LogError("No more than two gamepads can be plugged in at a time");
                return;
            }

            ButtonControl buttonControl = new ButtonControl();

            input.P1_Stick = allGamepads[0].leftStick;
            input.P1_ActionButton = new GameButton(allGamepads[0].rightTrigger, "", true);
            input.P1_DashButton = new GameButton(allGamepads[0].leftTrigger, "", true);
            input.P1_ConfirmButton = new GameButton(allGamepads[0].aButton, "", true);

            input.P2_Stick = allGamepads[1].leftStick;
            input.P2_ActionButton = new GameButton(allGamepads[1].rightTrigger, "", true);
            input.P2_DashButton = new GameButton(allGamepads[1].leftTrigger, "", true);
            input.P2_ConfirmButton = new GameButton(allGamepads[1].aButton, "", true);

        }

        SetGlobalInput(input);
    }

    private void GetArcadeJoysticks(out Joystick p1_Stick, out Joystick p2_Stick)
    {
        p1_Stick = null;
        p2_Stick = null;

        //fill joystick objects
        var joysticks = Joystick.all;

        if (joysticks.Count > 0)
        {
            for (int i = 0; i < joysticks.Count; i++)
            {
                if (joysticks[i].GetType() == typeof(Joystick))
                {
                    Joystick joystick = (Joystick)joysticks[i];

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

    private void SetLastInputTime(bool isPressed)
    {
        //set last action time
        m_LastInputTime = isPressed ? Time.time : m_LastInputTime;
    }

    public void ForceInput()
    {
        SetLastInputTime(true);
    }

    ////////////////////INPUT GETTERS////////////////////////////
    public Vector2 GetStickValue(bool isPlayerOne)
    {
        StickControl stick = isPlayerOne ? m_GlobalInput.P1_Stick : m_GlobalInput.P2_Stick;

        if (stick == null) { return Vector2.zero; }
      
        Vector2 value = stick.ReadValue();

        if (Mathf.Abs(value.x) <= m_DeadZone) { value.x = 0; }
        if (Mathf.Abs(value.y) <= m_DeadZone) { value.y = 0; }

        bool isMoved = value.magnitude > 0.5f ? true : false;
        SetLastInputTime(isMoved);

        return value;
    }

    public bool IsActionButtonPressedDown(bool isPlayerOne)
    {
        GameButton gameButton = isPlayerOne ? m_GlobalInput.P1_ActionButton : m_GlobalInput.P2_ActionButton;

        if (gameButton == null) { return false; }

        bool isPressed = gameButton.GetButtonInput();

        SetLastInputTime(isPressed);

        return isPressed;
    }

    public bool IsDashButtonPressedDown(bool isPlayerOne)
    {
        GameButton gameButton = isPlayerOne ? m_GlobalInput.P1_DashButton : m_GlobalInput.P2_DashButton;

        if (gameButton == null) { return false; }

        bool isPressed = gameButton.GetButtonInput();

        SetLastInputTime(isPressed);

        return isPressed;
    }

    public bool IsConfirmButtonPressedDown(bool isPlayerOne)
    {
        GameButton gameButton = isPlayerOne ? m_GlobalInput.P1_ConfirmButton : m_GlobalInput.P2_ConfirmButton;

        if (gameButton == null) { return false; }

        bool isPressed = gameButton.GetButtonInput();

        SetLastInputTime(isPressed);

        return isPressed;
    }

    public bool IsCoinButtonPressedDown()
    {
        bool isPressed = Input.GetButtonDown("ArcadeSpecialInputThree") | Input.GetButtonDown("ArcadeSpecialInputTwo") | Input.GetButtonDown("ArcadeSpecialInputOne");

        SetLastInputTime(isPressed);

        return isPressed;
    }

    public bool IsStartButtonPressedDown()
    {
        bool isPressed = Input.GetButtonDown("StartGame");

        SetLastInputTime(isPressed);

        return isPressed;
    }

    public float GetLastInputTime()
    {
        return m_LastInputTime;
    }

}

public struct GameInput
{
    public StickControl P1_Stick;
    public GameButton P1_ActionButton;
    public GameButton P1_DashButton;
    public GameButton P1_ConfirmButton;

    public StickControl P2_Stick;
    public GameButton P2_ActionButton;
    public GameButton P2_DashButton;
    public GameButton P2_ConfirmButton;
}

//needed since gamepads need a buttonControl
public class GameButton
{
    public ButtonControl m_ButtonControl;
    public string m_ButtonInputName;

    private bool m_UseButtonControl;

    public GameButton (ButtonControl buttonControl, string alternateButtonName, bool useButtonControl)
    {
        m_ButtonControl = buttonControl;
        m_ButtonInputName = alternateButtonName;
        m_UseButtonControl = useButtonControl;
    }

    public bool GetButtonInput()
    {
        bool input = false;
        if (m_UseButtonControl)
        {
            input = m_ButtonControl.isPressed;
        }
        else
        {
            input = Input.GetButtonDown(m_ButtonInputName);
        }

        return input;
    }
}
