using System.Collections.Generic;
using UnityEngine;

public class GravityFieldSpherical : GravityField
{
    [SerializeField] private bool Inversed = false;
    private float innerRadius;
    private float gravityRadius;
    SphereCollider sphereCollider;
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    protected override bool CheckCollisionOverlap(out List<Rigidbody> rbs)
    {
        bool collided = false;
        rbs = new List<Rigidbody>();
        Collider[] hits = Physics.OverlapSphere(transform.position, sphereCollider.radius * Mathf.Max(sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.y, sphereCollider.transform.lossyScale.z));
        foreach (Collider x in hits)
        {
            if (x.attachedRigidbody != null)
            {
                rbs.Add(x.attachedRigidbody);
                collided = true;
            }
        }
        return collided;
    }

    public override Vector3 CalculateGravityVector(Transform tr)
    {
        Vector3 vec;
        if (!Inversed)
            vec = (transform.position - tr.position).normalized;
        else
            vec = -(transform.position - tr.position).normalized;

        return vec;
    }

    protected override void ApplyRigidbodyGravity(Rigidbody rb)
    {
        rb.AddForce((transform.position - rb.position).normalized * rbGravity);
    }

}
