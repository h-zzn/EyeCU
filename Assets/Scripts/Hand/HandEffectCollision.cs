using UnityEngine;

public class HandEffectCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HandEffect"))
        {
            Debug.Log("Collision Detected with: " + other.name);

            // Deactivate children of this GameObject
            DeactivateChildren(this.gameObject);

            // Deactivate children of the colliding GameObject
            DeactivateChildren(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HandEffect"))
        {
            ReactivateChildren(this.gameObject);

            ReactivateChildren(other.gameObject);
        }
    }

    private void DeactivateChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Debug.Log("Deactivating: " + child.name);
            child.gameObject.SetActive(false);
        }
    }

    private void ReactivateChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
