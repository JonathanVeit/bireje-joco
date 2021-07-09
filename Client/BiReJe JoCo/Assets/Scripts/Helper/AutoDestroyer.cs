using UnityEngine;

[ExecuteInEditMode]
public class AutoDestroyer : MonoBehaviour
{
    void Start()
    {
        this.gameObject.hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy;

        if (Application.isPlaying)
            Destroy(this.gameObject);
    }
}
