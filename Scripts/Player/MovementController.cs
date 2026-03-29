using UnityEngine;

public class MovementController : MonoBehaviour
{
    private CapsuleCollider capsuleCollider;
    [SerializeField] private bool GravityEnabled = true;
    [SerializeField] private bool GlobalGravityEnabled = true;

    [SerializeField] private float gravity = 30f;

    private float currentgravityAlignSpeed = 0f;
    private float targetGravityAlignSpeed = 0.5f;
    [SerializeField] private float gravityAlignMaxDelta = 0.3f;
    
    [SerializeField] private float maxClimbAngle = 55;

    private bool InGravityField = false;

    private int maxRecursion = 3;
    private int recursionDepth;
    float offset = 0.01f;

    Vector3 externalVelocity = Vector3.zero;
    Vector3 vel;
    Vector3 gravityVec = Vector3.down;

    int layerMask;

    void Awake()
    {
        layerMask = ~LayerMask.GetMask("Player");
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public Vector3 Move(Vector3 velocity)
    {
        if (GravityEnabled)
        {
            if (!InGravityField && GlobalGravityEnabled)
                gravityVec = Vector3.down;
            if (!InGravityField && !GlobalGravityEnabled)
                gravityVec = Vector3.zero;
                
            velocity += gravityVec * gravity * Time.fixedDeltaTime;

            GravityOrientation();
            ResolvePenetration();
        }

        velocity += externalVelocity;
        externalVelocity = Vector3.zero;
        
        Vector3 displacement = velocity * Time.fixedDeltaTime;

        
        Vector3 up = transform.up;
        Vector3 verticalDisp = Vector3.Project(displacement, up);
        Vector3 lateralDisp = displacement - verticalDisp;

        // Calculate the lateral displacement after collisions
        recursionDepth = 0;
        Vector3 resolvedLateral = CollideAndSlide(transform.position, lateralDisp, false);

        // Calculate the vertical displacement after collisions. This separation makes moving on slopes and stairs better
        recursionDepth = 0;
        Vector3 resolvedVertical = CollideAndSlide(transform.position + resolvedLateral, verticalDisp, true);


        transform.position += resolvedVertical + resolvedLateral;

        
        Vector3 totalResolved = resolvedLateral + resolvedVertical;


        vel = totalResolved / Time.fixedDeltaTime;
        return vel;
    }

    // Collision detection and handling function
    private Vector3 CollideAndSlide(Vector3 pos, Vector3 vel, bool GravityPass)
    {
        if (recursionDepth > maxRecursion)
            return Vector3.zero;

        float dist = vel.magnitude + offset;
        
        if (Physics.CapsuleCast(
            pos + transform.up * (capsuleCollider.height / 2 - capsuleCollider.radius),
            pos - transform.up * (capsuleCollider.height / 2 - capsuleCollider.radius),
            capsuleCollider.radius, vel.normalized, out RaycastHit hit, dist,
            layerMask, QueryTriggerInteraction.Ignore))
        {

            Vector3 newVel = vel.normalized * (hit.distance - offset);
            float angle = Vector3.Angle(transform.up, hit.normal);

            if (newVel.magnitude <= offset)
                newVel = Vector3.zero;


            Vector3 newPos = pos + newVel;

            Vector3 vecOnPlane = Vector3.ProjectOnPlane(vel - newVel, hit.normal);

            if (GravityPass && angle < maxClimbAngle)
                return newVel;

            recursionDepth++;
            return newVel + CollideAndSlide(newPos, vecOnPlane, GravityPass);
        }
        return vel;
    }


    private void GravityOrientation()
    {
        // On entering a gravity field, gravity align speed is set to zero in Gravity Controller, the following code
        // gradually increases the align speed back. It makes it feel better going in and out of gravity fields.
        if (currentgravityAlignSpeed != targetGravityAlignSpeed)
        {
            currentgravityAlignSpeed = Mathf.MoveTowards(currentgravityAlignSpeed, targetGravityAlignSpeed, gravityAlignMaxDelta * Time.fixedDeltaTime);
        }

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -gravityVec) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentgravityAlignSpeed);
    }


    // When the collider is rotated for gravity orientation, it may overlap unpredictably with environmental colliders, 
    // causing player to fall through the environment. This function helps with it.
    private void ResolvePenetration()
    {
        Collider[] overlap = Physics.OverlapCapsule(
            transform.position + transform.up * (capsuleCollider.height / 2 - capsuleCollider.radius), 
            transform.position - transform.up * (capsuleCollider.height / 2 - capsuleCollider.radius), 
            capsuleCollider.radius, layerMask, QueryTriggerInteraction.Ignore);
        if (overlap.Length > 0)
        {
            foreach (Collider x in overlap)
            {
                if (x == capsuleCollider)
                    continue;
                
                if (Physics.ComputePenetration(
                    capsuleCollider, transform.position, transform.rotation,
                    x, x.transform.position, x.transform.rotation,
                    out Vector3 dir, out float dis))
                {
                    transform.position += dir * (dis + 0.01f);
                }
            }
        }
    }

    


    public bool GroundCheck()
    {
        if (Physics.SphereCast(transform.position, capsuleCollider.radius, -transform.up, 
        out RaycastHit hit, 0.6f, layerMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }
    public bool GroundCheck(out RaycastHit hit)
    {
        if (Physics.SphereCast(transform.position, capsuleCollider.radius, -transform.up, 
        out hit, 0.6f, layerMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }

    public void AddVelocity(Vector3 x)
    {
        externalVelocity += x;
    }

    public Vector3 GetVelocity()
    {
        return vel;
    }

    public void ResetVerticalVelocity()
    {
        externalVelocity -= Vector3.Project(vel, transform.up);
    }

    public void ResetVelocity()
    {
        externalVelocity -= vel;
    }

    public void SetGravityVec(Vector3 x)
    {
        gravityVec = x;
    }

    public void SetGravityAlignSpeed(float x)
    {
        currentgravityAlignSpeed = x;
    }


    public float GetGravity()
    {
        return gravity;
    }

    public void SetGravity(float gravity_)
    {
        gravity = gravity_;
    }

    public void SetInGravityField(bool x)
    {
        InGravityField = x;
    }

    public bool GetInGravityField()
    {
        return InGravityField;
    }
}
