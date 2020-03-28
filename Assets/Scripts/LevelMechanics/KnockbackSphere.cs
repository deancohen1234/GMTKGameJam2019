using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class KnockbackSphere : MonoBehaviour
{
    public Action<PlayerController, Vector3> m_OnSphereHit;
    private Collider m_IgnoringCollider;

    public void CreateSphere(float radius, float lifetime, Collider ignoredCollider)
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        transform.localScale = new Vector3(radius, radius, radius);
        m_IgnoringCollider = ignoredCollider;
        collider.radius = 0.5f;

        Invoke("DestroyMyself", lifetime);
    }
    
    private void DestroyMyself()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.Equals(m_IgnoringCollider))
        {
            if (other.tag == "Player")
            {
                //send in player that was hit and sphere origin
                m_OnSphereHit?.DynamicInvoke(other.gameObject.GetComponent<PlayerController>(), transform.position);
            }
        }
    }

    private void OnDestroy()
    {
        m_OnSphereHit = null;
    }
}
