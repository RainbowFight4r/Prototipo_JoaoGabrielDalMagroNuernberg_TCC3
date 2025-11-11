using UnityEngine;

public class LanternIndependentRotation : MonoBehaviour
{
    private Transform parentTransform;
    private Vector3 lastParentScale;

    void Start()
    {
        if (transform.parent != null)
        {
            parentTransform = transform.parent;
            lastParentScale = parentTransform.localScale;
        }
    }

    void LateUpdate()
    {
        if (parentTransform != null)
        {
            if (parentTransform.localScale.x != lastParentScale.x)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x *= -1;
                transform.localScale = currentScale;

                lastParentScale = parentTransform.localScale;
            }
        }
    }
}