using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Using custom OnTrigger calls, manages the gravity fields and sets gravity vector
/// </summary>

public class GravityController : MonoBehaviour, ICustomTriggerReceiver
{
    MovementController mc = null;
    List<Collider> gravityFields = new List<Collider>();
    Collider prioritizedField = null;


    void Start()
    {
        mc = GetComponent<MovementController>();
    }

    public void OnCustomTriggerEnter(Collider other)
    {
        if (other.CompareTag("GravityField"))
        {
            if (!gravityFields.Contains(other))
                gravityFields.Add(other);

            if (prioritizedField == null || prioritizedField.GetComponent<GravityFieldSpherical>() != null)
            {
                prioritizedField = other;
                mc.SetInGravityField(true);
                mc.SetGravityAlignSpeed(0);
                mc.SetGravityVec(other.GetComponent<GravityField>().CalculateGravityVector(transform));
            }

        }
    }

    public void OnCustomTriggerStay(Collider other)
    {
        if (other.CompareTag("GravityField"))
        {
            if (other == prioritizedField)
            {
                mc.SetInGravityField(true);
                mc.SetGravityVec(other.GetComponent<GravityField>().CalculateGravityVector(transform));
            }
        }
    }

    public void OnCustomTriggerExit(Collider other)
    {
        if (other.CompareTag("GravityField"))
        {
            gravityFields.Remove(other);

            if (other == prioritizedField)
            {
                if (gravityFields.Count > 0)
                {
                    prioritizedField = gravityFields[0];
                    mc.SetGravityAlignSpeed(0);
                    mc.SetGravityVec(prioritizedField.GetComponent<GravityField>().CalculateGravityVector(transform));
                }
                else
                {
                    prioritizedField = null;
                    mc.SetInGravityField(false);
                    mc.SetGravityAlignSpeed(0);
                }
            }
        }
    }
}
