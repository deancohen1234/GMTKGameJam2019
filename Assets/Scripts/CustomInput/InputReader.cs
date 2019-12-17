using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private string m_OnInputPressedOutput;

    private Joystick m_P1_Joystick;
    private Joystick m_P2_Joystick;
    // Start is called before the first frame update
    void Start()
    {
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

                    if (m_P1_Joystick == null)
                    {
                        m_P1_Joystick = joystick;
                        continue;
                    }

                    if (m_P1_Joystick.Equals(joystick))
                    {
                        //we are referencing P1 Joystick and it already has been filled
                        continue;
                    }
                    else if (m_P2_Joystick == null)
                    {
                        m_P2_Joystick = joystick;
                    }
                    else
                    {
                        //p1 and p2 joysticks are filled and there is an extra joystick
                    }
                    
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (m_P1_Joystick != null && m_P2_Joystick != null)
        {
            m_OnInputPressedOutput = "P1: " + m_P1_Joystick.stick.ReadValue() + "\nP2: " + m_P2_Joystick.stick.ReadValue();
        }

        /*
        if (Input.GetButton("Arcade_P1_LR"))
        {
            float num = Input.GetAxis("Arcade_P1_LR");
            m_OnInputPressedOutput = "HellYeah!: " + num.ToString();
        }
        else if (Input.GetButton("Arcade_P1_UD"))
        {
            float num = Input.GetAxis("Arcade_P1_UD");
            m_OnInputPressedOutput = "HellYeah!: " + num.ToString();
        }
        else if (Input.GetButton("Arcade_P2_LR"))
        {
            float num = Input.GetAxis("Arcade_P2_LR");
            m_OnInputPressedOutput = "HellYeah!: " + num.ToString();
        }
        else if (Input.GetButton("Arcade_P2_UD"))
        {
            float num = Input.GetAxis("Arcade_P2_UD");
            m_OnInputPressedOutput = "HellYeah!: " + num.ToString();
        }
        else
        {
            m_OnInputPressedOutput = "Nada...";
        }*/

        //m_OnInputPressedOutput = p1_input.Direction.ToString() + "\n" + p2_input.Direction.ToString();

        //m_OnInputPressedOutput = Input.GetAxis(ApplicationSettings.m_GlobalInput.P2_DashButton).ToString();

    }

    /*
    private ArcadeInput GetArcadeInput(bool isPlayerOne)
    {
        string horizontalInputStr = isPlayerOne ? ApplicationSettings.m_GlobalInput.P1_HorizonalAxis : ApplicationSettings.m_GlobalInput.P2_HorizonalAxis;
        string verticalInputStr = isPlayerOne ? ApplicationSettings.m_GlobalInput.P1_VerticalAxis : ApplicationSettings.m_GlobalInput.P2_VerticalAxis;

        int horizonatalInput = (int)Input.GetAxis(horizontalInputStr);
        int verticalInput = (int)Input.GetAxis(verticalInputStr);
        Vector2 direction = GetArcadeInputDirection(horizonatalInput, verticalInput);


        ArcadeInput input;
        input.HorizontalInput = horizonatalInput;
        input.VerticalInput = verticalInput;
        input.Direction = direction;

        return input;
    }*/

    private Vector2 GetArcadeInputDirection(int horizontalInput, int verticalInput)
    {
        Vector2 dir = new Vector2((float)horizontalInput, (float)verticalInput);

        //while this math is awesome and fun, normalizing the vector does the same thing and is eaiser
        /*
        //we ar in a diagonal vector
        if (dir.sqrMagnitude > 1)
        {
            //this keeps all vectors at the same magnitude
            dir *= Mathf.Sin(Mathf.PI * 0.25f); //45 degrees or (root 2)/2
        }*/
        dir.Normalize();

        return dir;
    }

    private void OnGUI()
    {
        string allDevicesList = "";
        for (int i = 0; i < InputSystem.devices.Count; i++)
        {
            InputDevice id = InputSystem.devices[i];
            allDevicesList += "[" + id.displayName + "]";
        }
        GUI.Label(new Rect(10, 10, 300, 100), "Input: " + allDevicesList);

        GUI.Label(new Rect(10, 200, 300, 100), m_OnInputPressedOutput); 
    }
}


public struct ArcadeInput
{
    public int HorizontalInput;
    public int VerticalInput;

    public Vector2 Direction;
}
