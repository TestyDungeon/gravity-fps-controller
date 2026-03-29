using UnityEngine;
using System.Collections.Generic;

public class GravityFieldCuboidal : GravityField
{
    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    protected override bool CheckCollisionOverlap(out List<Rigidbody> rbs)
    {
        bool collided = false;
        rbs = new List<Rigidbody>();

        Collider[] hits = Physics.OverlapBox(boxCollider.transform.TransformPoint(boxCollider.center), Vector3.Scale(boxCollider.size * 0.5f, boxCollider.transform.lossyScale), boxCollider.transform.rotation);
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

    protected override void ApplyRigidbodyGravity(Rigidbody rb)
    {
        rb.AddForce(-transform.up * rbGravity);
    }

    public override Vector3 CalculateGravityVector(Transform tr)
    {
        return -transform.up;
    }
}
