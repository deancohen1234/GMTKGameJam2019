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
    private Collider[] arrowOverlapColliders;
    private RaycastHit[] arrowWallHits;
    private RaycastHit[] arrowPreWallHits;

    private float launchSpeed;
    private float pickSqrSpeedThreshold;
    private int freeBouncesRemaining; //when this is at 0, each bounce will add drag

    private Rigidbody body;
    private WallCollisionBuffer wallCollisionBuffer;

    private Vector3 lastBufferedNormal;

    private int DELETEME = 0;
    private bool isLaunched = false;

    private const float VERTICALDOTTHRESHOLD = 0.90f; //make sure we don't get collisions from the ground

    #region Monobehavior
    private void OnEnable()
    {
        body = GetComponent<Rigidbody>();

        Initialize();
    }

    protected override void Update()
    {
        if (!Physics.autoSimulation)
        {
            if (Input.GetKey(KeyCode.Alpha9))
            {
                RunPhysics();
                Physics.Simulate(Time.fixedDeltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        if (Physics.autoSimulation)
        {
            RunPhysics();
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

        if (arrowWallHits == null || arrowWallHits.Length > 0)
        {
            arrowWallHits = new RaycastHit[5];
        }

        if (arrowOverlapColliders == null || arrowOverlapColliders.Length > 0)
        {
            arrowOverlapColliders = new Collider[5];
        }

        if (arrowPreWallHits == null || arrowPreWallHits.Length > 0)
        {
            arrowPreWallHits = new RaycastHit[5];
        }

        pickSqrSpeedThreshold = m_PickupSpeedThreshold * m_PickupSpeedThreshold;
    }

    private void RunPhysics()
    {
        DELETEME++;
        if (isLaunched)
        {
            if (wallCollisionBuffer.isBuffered)
            {
                EmptyAndUseWallCollisionBuffer(ref wallCollisionBuffer);
            }

            CheckArrowCollision();

            if (body.velocity.sqrMagnitude <= pickSqrSpeedThreshold && !wallCollisionBuffer.isBuffered)
            {
                OnSpeedBelowThreshold();
            }
        }
    }

    public void Shoot(Vector3 direction)
    {
        //Physics.autoSimulation = false;

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

        wallCollisionBuffer.isBuffered = false;
    }

    private void OnSpeedBelowThreshold()
    {
        SetCollidersActive(true);
        isLaunched = false;
    }


    #region Arrow Collision
    private void CheckArrowCollision()
    {
        //check if we hit player
        PlayerSphereCheck();

        Vector3 direction = Vector3.ProjectOnPlane(m_Rigidbody.velocity.normalized, Vector3.up);

        //there is something right in front of us so bail, this guy's got it
        if (PreRaycastCheck(direction)) { return; }

        Debug.DrawLine(transform.position, transform.position + direction * m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime, Color.white);
        //Debug.Break();
        SphereCastCheck(direction);
    }

    private void PlayerSphereCheck()
    {
        int playerHits = Physics.OverlapSphereNonAlloc(transform.position, m_HitboxRadius, arrowHitColliders, m_HitboxMask);
        if (playerHits > 0)
        {
            for (int i = 0; i < playerHits; i++)
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
    }

    //make sure something isn't immediately infront of arrow
    private bool PreRaycastCheck(Vector3 direction)
    {
        int wallHits = Physics.RaycastNonAlloc(transform.position, direction, arrowPreWallHits, m_SpherecastRadius * raycastDistanceModifier * 1.25f, m_BouncingHitBoxMask);

        if (wallHits > 0)
        {
            for (int i = 0; i < wallHits; i++)
            {
                if (arrowPreWallHits[i].collider != null && arrowPreWallHits[i].distance > 0)
                {
                    Debug.Log("Arrow Wall Pre Hit: " + arrowPreWallHits[i].collider.name);
                    BufferSurfaceCollision(arrowPreWallHits[i].point, arrowPreWallHits[i].normal, arrowPreWallHits[i].collider, arrowPreWallHits[i].collider.name);
                    return true;
                }
            }
        }

        return false;

    }

    private void SphereCastCheck(Vector3 direction)
    {
        float castDist = (m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime) + m_SpherecastRadius + Mathf.Epsilon;
        //check for wall collisions
        //int wallHits = Physics.RaycastNonAlloc(transform.position, direction, arrowHitWallHits, m_Rigidbody.velocity.magnitude * raycastDistanceModifier * Time.fixedDeltaTime, m_BouncingHitBoxMask);
        int wallHits = Physics.SphereCastNonAlloc(transform.position, m_SpherecastRadius, direction, arrowWallHits, castDist, m_BouncingHitBoxMask);
        Debug.Log("Wall Hits: " + wallHits);
        if (wallHits > 0)
        {
            int shortestDistIndex = -1;
            for (int i = 0; i < wallHits; i++)
            {
                if (arrowWallHits[i].collider != null && arrowWallHits[i].distance > 0)
                {
                    float shortestDist = float.MaxValue;

                    if (arrowWallHits[i].distance < shortestDist)
                    {
                        shortestDistIndex = i;
                        shortestDist = arrowWallHits[i].distance;
                    }
                }
            }

            if (shortestDistIndex > -1)
            {
                Vector3 normal = Vector3.ProjectOnPlane(arrowWallHits[shortestDistIndex].normal, Vector3.up);
                Vector3 point = arrowWallHits[shortestDistIndex].point;
                point.y = transform.position.y;

                if (normal != lastBufferedNormal)
                {
                    point = VerifyHitPosition(point);

                    Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                    arrowWallHits[shortestDistIndex].collider.gameObject.GetComponent<MeshRenderer>().material.color = randomColor;
                    BufferSurfaceCollision(point, normal, arrowWallHits[shortestDistIndex].collider, arrowWallHits[shortestDistIndex].collider.name);
                }
            }
        }
    }

    private bool OverlapSphereArrowCheck(Vector3 position, Collider ignoreCollider)
    {
        if (Physics.OverlapSphereNonAlloc(position, m_SpherecastRadius + Mathf.Epsilon, arrowOverlapColliders, m_BouncingHitBoxMask) > 0)
        {
            for (int i = 0; i < arrowOverlapColliders.Length; i++)
            {
                if (arrowOverlapColliders[i] != null)
                {
                    Debug.Log("Give me Names Damnit: " + arrowOverlapColliders[i].name);
                    if (!ignoreCollider.Equals(arrowOverlapColliders[i])) 
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private Vector3 VerifyHitPosition(Vector3 foundPos)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, (foundPos - transform.position).normalized, out hit, 100f, m_BouncingHitBoxMask))
        {
            return hit.point;
        }
        else
        {
            Debug.Log("Yikes this ain't good");
            return foundPos;
        }
    }

    //store for 1 physics timestep, the position and normal that will happen next time step

    //NOTE: May need to do an overlap sphere at the position, to make sure we aren't too close to another wall
    //Could be that a raycast is better too
    private void BufferSurfaceCollision(Vector3 position, Vector3 normal, Collider collider, string name)
    {
        wallCollisionBuffer.velocity = m_Rigidbody.velocity;
        wallCollisionBuffer.normal = normal;
        wallCollisionBuffer.collider = collider;
        wallCollisionBuffer.isBuffered = true;

        lastBufferedNormal = normal;

        //body.MovePosition(position);
        body.velocity = Vector3.zero;
        body.position = position;

        Vector3 reflectedVel = Vector3.Reflect(wallCollisionBuffer.velocity, normal);
        Debug.DrawLine(position, position + reflectedVel * raycastDistanceModifier * Time.fixedDeltaTime, Color.green);

        //Debug.Log("Buffering Hit: " + name + " " + DELETEME);

    }

    private void EmptyAndUseWallCollisionBuffer(ref WallCollisionBuffer buffer)
    {
        float dot = Vector3.Dot(buffer.normal, Vector3.up);

        if (dot <= VERTICALDOTTHRESHOLD)
        {
            freeBouncesRemaining--;

            if (freeBouncesRemaining < 0)
            {
                body.drag += m_DragPerBounce;
            }

            if (buffer.normal != Vector3.zero)
            {
                body.velocity = Vector3.Reflect(buffer.velocity, buffer.normal);
            }
            else
            {
                //just reverse the direction
                body.velocity *= -1f;
                Debug.Log("reversing direction");
            }

            //empty buffer
            buffer.isBuffered = false;
            buffer.collider = null;
        }

    }
    #endregion

    private void HitPlayer(PlayerController player)
    {
        player.AttemptAttack(m_OwningPlayer);
    }
}

public struct WallCollisionBuffer
{
    public bool isBuffered;
    public Vector3 velocity;
    public Vector3 normal;
    public Collider collider;
}
