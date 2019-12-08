using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApplicationSettings : MonoBehaviour
{
    public SystemInput m_SystemInput;
    public bool m_ArcadeMode;

    public static GameInput m_GlobalInput; //input used for entire game
    // Start is called before the first frame update
    void Awake()
    {
        QualitySettings.vSyncCount = 0; //disable vsync
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        SetupInput();
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.isPressed)
        {
            Application.Quit();
        }
    }

    private void SetupInput()
    {
        //setup input for gamemode (arcade vs normal)
        GameInput input;

        if (m_ArcadeMode)
        {
            input.P1_HorizonalAxis = m_SystemInput.Arcade_P1_HorizonalAxis;
            input.P1_VerticalAxis = m_SystemInput.Arcade_P1_VerticalAxis;
            input.P1_ActionButton = m_SystemInput.Arcade_P1_ActionButton;
            input.P1_DashButton = m_SystemInput.Arcade_P1_DashButton;
            input.P1_ConfirmButton = m_SystemInput.Arcade_P1_ConfirmButton;

            input.P2_HorizonalAxis = m_SystemInput.Arcade_P2_HorizonalAxis;
            input.P2_VerticalAxis = m_SystemInput.Arcade_P2_VerticalAxis;
            input.P2_ActionButton = m_SystemInput.Arcade_P2_ActionButton;
            input.P2_DashButton = m_SystemInput.Arcade_P2_DashButton;
            input.P2_ConfirmButton = m_SystemInput.Arcade_P2_ConfirmButton;

            input.UsingArcadeControls = true;
        }
        else
        {
            input.P1_HorizonalAxis = m_SystemInput.Normal_P1_HorizonalAxis;
            input.P1_VerticalAxis = m_SystemInput.Normal_P1_VerticalAxis;
            input.P1_ActionButton = m_SystemInput.Normal_P1_ActionButton;
            input.P1_DashButton = m_SystemInput.Normal_P1_DashButton;
            input.P1_ConfirmButton = m_SystemInput.Normal_P1_ConfirmButton;

            input.P2_HorizonalAxis = m_SystemInput.Normal_P2_HorizonalAxis;
            input.P2_VerticalAxis = m_SystemInput.Normal_P2_VerticalAxis;
            input.P2_ActionButton = m_SystemInput.Normal_P2_ActionButton;
            input.P2_DashButton = m_SystemInput.Normal_P2_DashButton;
            input.P2_ConfirmButton = m_SystemInput.Normal_P2_ConfirmButton;

            input.UsingArcadeControls = false;
        }

        UpdateInput(input);
    }

    private void UpdateInput(GameInput gameInput)
    {
        m_GlobalInput.P1_HorizonalAxis = gameInput.P1_HorizonalAxis;
        m_GlobalInput.P1_VerticalAxis = gameInput.P1_VerticalAxis;
        m_GlobalInput.P1_ActionButton = gameInput.P1_ActionButton;
        m_GlobalInput.P1_DashButton = gameInput.P1_DashButton;
        m_GlobalInput.P1_ConfirmButton = gameInput.P1_ConfirmButton;

        m_GlobalInput.P2_HorizonalAxis = gameInput.P2_HorizonalAxis;
        m_GlobalInput.P2_VerticalAxis = gameInput.P2_VerticalAxis;
        m_GlobalInput.P2_ActionButton = gameInput.P2_ActionButton;
        m_GlobalInput.P2_DashButton = gameInput.P2_DashButton;
        m_GlobalInput.P2_ConfirmButton = gameInput.P2_ConfirmButton;

        m_GlobalInput.UsingArcadeControls = gameInput.UsingArcadeControls;
    }

}

public struct GameInput
{
    public string P1_HorizonalAxis;
    public string P1_VerticalAxis;
    public string P1_ActionButton;
    public string P1_DashButton;
    public string P1_ConfirmButton;

    public string P2_HorizonalAxis;
    public string P2_VerticalAxis;
    public string P2_ActionButton;
    public string P2_DashButton;
    public string P2_ConfirmButton;

    public bool UsingArcadeControls;
}
