using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float m_MoveSpeed = 5.0f;

    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Debug.Log("X: " + x + "Y: " + y);

        Vector3 inputVelocity = new Vector3(x, 0f, y) * Time.deltaTime * m_MoveSpeed;

        m_Rigidbody.velocity = inputVelocity;
    }
}
