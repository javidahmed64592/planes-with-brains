using UnityEngine;

public class PlaneCollider : MonoBehaviour
{
    string collideTag = "Collide";

    PlaneController plane;

    private void Awake()
    {
        plane = GetComponentInParent<PlaneController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(collideTag))
        {
            plane.Kill();
        }
    }
}
