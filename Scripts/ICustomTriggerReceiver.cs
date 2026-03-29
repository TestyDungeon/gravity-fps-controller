using UnityEngine;

public interface ICustomTriggerReceiver
{
    void OnCustomTriggerEnter(Collider other);

    void OnCustomTriggerStay(Collider other);

    void OnCustomTriggerExit(Collider other);
}
