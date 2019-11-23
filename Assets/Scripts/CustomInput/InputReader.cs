using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private string m_OnInputPressedOutput;
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
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

        ArcadeInput p1_input = GetArcadeInput(true);
        ArcadeInput p2_input = GetArcadeInput(false);

        m_OnInputPressedOutput = p1_input.Direction.ToString() + "\n" + p2_input.Direction.ToString();

    }

    private ArcadeInput GetArcadeInput(bool isPlayerOne)
    {
        string horizontalInputStr = isPlayerOne ? "Arcade_P1_LR" : "Arcade_P2_LR";
        string verticalInputStr = isPlayerOne ? "Arcade_P1_UD" : "Arcade_P2_UD";

        int horizonatalInput = (int)Input.GetAxis(horizontalInputStr);
        int verticalInput = (int)Input.GetAxis(verticalInputStr);
        Vector2 direction = GetArcadeInputDirection(horizonatalInput, verticalInput);


        ArcadeInput input;
        input.HorizontalInput = horizonatalInput;
        input.VerticalInput = verticalInput;
        input.Direction = direction;

        return input;
    }

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
