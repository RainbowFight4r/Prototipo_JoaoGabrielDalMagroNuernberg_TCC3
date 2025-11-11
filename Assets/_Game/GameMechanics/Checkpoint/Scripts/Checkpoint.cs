using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        if (CheckpointSystem.Instance != null)
        {
            CheckpointSystem.Instance.SetCheckpoint(transform.position);
        }

        isActivated = true;

        Debug.Log("Checkpoint activated at: " + transform.position);
    }
}