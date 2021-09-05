using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BouncyArrow : DivineWeapon
{
    [Header("Move Properties")]
    public float m_StartingSpeed = 5f;
    public int m_StartingFreeBounces = 1;
    public float m_DragPerBounce = 2.5f;
    public float m_PickupSpeedThreshold = 1f;

    [Header("Detection Properties")]
    public LayerMask m_HitboxMask;
    public LayerMask m_BouncingHitBoxMask;
    public float raycastDistanceModifier = 1f;
    public float m_HitboxRadius = 0.5f;
    public float m_SpherecastRadius = 0.5f;

    private Collider[] arrowHitColliders;
    private RaycastHit[] arrowHitWallHits;

    private float launchSpeed;
    private float pickSqrSpeedThreshold;
    private int freeBouncesRemaining; //when this is at 0, each bounce will add drag

    private Rigidbody body;

    private bool isLaunched = false;

    private const float VERTICALDOTTHRESHOLD = 0.90f; //make sure we don't get collisions from the ground

    #region Monobehavior
    private void OnEnable()
    {
        body = GetComponent<Rigidbody>();

        Initialize();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.contacts.Length > 0)
            {
                OnSurfaceCollision(collision.contacts[0].normal);
            }
            else
            {
                OnSurfaceCollision(Vector3.zero);
            }
        }
        else
        {
            OnSurfaceCollision(Vector3.zero);
        }

    }

    private void FixedUpdate()
    {
        if (isLaunched)
        {
            CheckArrowCollision();

            if (body.velocity.sqrMagnitude <= pickSqrSpeedThreshold)
            {
                SetCollidersActive(true);
                isLaunched = false;
            }
        }
    }

    #endregion

    #region Weapon Overrides

    public override void OnAttackStart()
    {
        base.OnAttackStart();

        m_OwningPlayer.DropWeapon(false); //need to drop/throw weapon

        Shoot(m_OwningPlayer.GetMoveDirection());
    }

    public override void OnAttackEnd()
    {
        base.OnAttackEnd();
    }

    public override void Drop(PlayerController lastControlledPlayer)
    {
        //make sure on player drop don't randomize weapon position
        SetWeaponActive(true);
    }

    #endregion

    private void Initialize()
    {
        if (arrowHitColliders == null || arrowHitColliders.Length > 0)
        {
            arrowHitColliders = new Collider[5];
        }

        if (arrowHitWallHits == null || arrowHitWallHits.Length > 0)
        {
            arrowHitWallHits = new RaycastHit[5];
        }

        pickSqrSpeedThreshold = m_PickupSpeedThreshold * m_PickupSpeedThreshold;
    }

    public void Shoot(Vector3 direction)
    {
        ResetSpeed();

        isLaunched = true;

        //disable pickup collider
        SetCollidersActive(false);

        transform.position = m_OwningPlayer.transform.position;

        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; //don't freeze X and Z position
        body.isKinematic = false;

        body.velocity = direction * launchSpeed;
    }

    public void ResetSpeed()
    {
        launchSpeed = m_StartingSpeed;
        freeBouncesRemaining = m_StartingFreeBounces;
        body.drag = 0; //only set drag on bounces
    }

    private void OnSurfaceCollision(Vector3 contactNormal)
    {
        float dot = Vector3.Dot(contactNormal, Vector3.up);

        if (dot <= VERTICALDOTTHRESHOLD)
        {
            freeBouncesRemaining--;

            if (freeBouncesRemaining < 0)
            {
                body.drag += m_DragPerBounce;
            }

            body.velocity = Vector3.Reflect(body.velocity, contactNormal);
        }
        
    }

    private void CheckArrowCollision()
    {
        //check if we hit player
        if (Physics.OverlapSphereNonAlloc(transform.position, m_HitboxRadius, arrowHitColliders, m_HitboxMask) > 0)
        {
            for (int i = 0; i < arrowHitColliders.Length; i++)
            {
                if (arrowHitColliders[i] != null)
                {
                    PlayerController player = arrowHitColliders[i].GetComponent<PlayerController>();
                    if (player != null && !player.Equals(m_OwningPlayer)) //make sure original shooter can't be hit by own arrow
                    {
                        HitPlayer(player);
                        break;
                    }
                }
            }
        }


        Vector3 direction = Vector3.ProjectOnPlane(m_Rigidbody.velocity.normalized, Vector3.up);
        //Debug.DrawLine(transform.position, transform.position + direction * m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime);
        //Debug.Break();
        //check for wall collisions
        //if (Physics.RaycastNonAlloc(transform.position, direction, arrowHitWallHits, m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime, m_BouncingHitBoxMask) > 0)
        if (Physics.SphereCastNonAlloc(transform.position, 0.25f, direction, arrowHitWallHits, m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime, m_BouncingHitBoxMask) > 0) 
        {
            for (int i = 0; i < arrowHitWallHits.Length; i++)
            {
                if (arrowHitWallHits[i].collider != null && arrowHitWallHits[i].distance > 0)
                {
                    Vector3 normal = Vector3.ProjectOnPlane(arrowHitWallHits[i].normal, Vector3.up);
                    Vector3 point = arrowHitWallHits[i].point;
                    point.y = transform.position.y;
                    //transform.position = point + normal * m_SpherecastRadius * 2f;
                    OnSurfaceCollision(normal);
                    break;
                }
            }
        }
    }

    private void HitPlayer(PlayerController player)
    {
        player.AttemptAttack(m_OwningPlayer);
    }
}
