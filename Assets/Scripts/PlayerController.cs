using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Move Properties")]
    public float m_MoveSpeed = 5.0f;

    [Header("Dash Properties")]
    public float m_DashSpeed = 15.0f;
    public float m_DisabledMovementTime = 0.10f; //in seconds

    private Rigidbody m_Rigidbody;

    private bool m_CanMove = true;

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

        Move(x, y);

        if (Input.GetAxis("LTrigger") >= 0.8f)
        {
            Vector3 direction = new Vector3(x, 0, y);
            Dash(direction.normalized);
        }
    }

    private void Move(float x, float y)
    {
        Vector3 inputVelocity = new Vector3(x, 0f, y) * m_MoveSpeed; //no delta time because physcis is doing that for us

        m_Rigidbody.velocity = inputVelocity;
    }

    private void Dash(Vector3 direction)
    {
        StartCoroutine(DisablePlayerMovement(m_DisabledMovementTime));

        m_Rigidbody.velocity = direction * m_DashSpeed;
    }

    private IEnumerator DisablePlayerMovement(float time)
    {
        m_CanMove = false;

        yield return new WaitForSeconds(time);

        m_CanMove = true;
    }
}
