using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//blocks player from passing through this collider if they haven't been whacked
[RequireComponent(typeof(Collider))]
public class JusticeGuard : MonoBehaviour
{
    public float m_ForceThreshold = 300f;
    public float m_BouncebackForce = 3.0f;
    public float m_BouncebackThreshold = 1.0f;

    public Color m_PlayerOneMemoryColor = Color.blue;
    public Color m_PlayerTwoMemoryColor = Color.red;

    public int m_Health = 3;

    private JusticeUser m_MemorizedPlayer;
    private Material m_WallMaterial;
    private Color m_DefaultColor;
    private int m_CurrentHealth = 0;

    private void Awake()
    {
        m_WallMaterial = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        m_CurrentHealth = m_Health;

        m_DefaultColor = m_WallMaterial.GetColor("_MainColor");
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Player")
        {
            JusticeUser user = collision.collider.GetComponent<JusticeUser>();

            if (user.GetIsHit())
            {
                //if player can't break through wall, then they are set in the memory to be able to break through later
                if (CanBreakThroughWall(user))
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
                }
                else
                {
                    SetPlayerMemory(user);
                }
            }
            else if (collision.impulse.magnitude > m_BouncebackThreshold)
            {
                Vector3 direction = collision.contacts[0].point - collision.collider.transform.position;
                direction = direction.normalized;

                collision.collider.gameObject.GetComponent<Rigidbody>().AddForce(-direction * m_BouncebackForce);
            }
        }
    }

    private void OnDestroy()
    {
        m_WallMaterial.SetColor("_MainColor", m_DefaultColor);
    }

    //returns true if successful
    private bool CanBreakThroughWall(JusticeUser user)
    {
        /*
        m_CurrentHealth--;

        if (m_CurrentHealth <= 0)
        {
            return true;
        }

        else
        {
            return false;
        }
        */

        return user.GetIsMemorized();
    }

    private PlayerType GetPlayerType(JusticeUser user)
    {
        PlayerController player = user.GetAssociatedPlayer();
        return player.m_PlayerNum;
    }

    private void SetPlayerMemory(JusticeUser user)
    {
        if (m_MemorizedPlayer != null)
        {
            m_MemorizedPlayer.SetIsMemorized(false);
            m_MemorizedPlayer = null;
        }

        user.SetIsMemorized(true);
        m_MemorizedPlayer = user;
  
        PlayerType playerType = user.GetAssociatedPlayer().m_PlayerNum;      
        if (playerType == PlayerType.Player1)
        {
            m_WallMaterial.SetColor("_MainColor", m_PlayerOneMemoryColor);
        }
        else if (playerType == PlayerType.Player2)
        {
            m_WallMaterial.SetColor("_MainColor", m_PlayerTwoMemoryColor);
        }
    }

    public void ResetWall()
    {
        m_WallMaterial.SetColor("_MainColor", m_DefaultColor);
        m_MemorizedPlayer = null;
    }
}
