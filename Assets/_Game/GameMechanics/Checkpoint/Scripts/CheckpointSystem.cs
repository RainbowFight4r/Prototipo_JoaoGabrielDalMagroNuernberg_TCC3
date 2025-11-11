using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem Instance { get; private set; }

    private Vector3 currentCheckpoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentCheckpoint = GameObject.FindGameObjectWithTag("Player").transform.position;
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        Debug.Log("Checkpoint set at: " + position);
    }

    public void RespawnPlayer(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        player.transform.position = currentCheckpoint;


        Debug.Log("Player respawned at checkpoint: " + currentCheckpoint);
    }

    public Vector3 GetCurrentCheckpoint()
    {
        return currentCheckpoint;
    }
}