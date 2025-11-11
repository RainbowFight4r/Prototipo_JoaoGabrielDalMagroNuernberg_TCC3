using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private PlayerMovement playerMovement;
    CapsuleCollider2D playerCollider;

    public float respawnDelay = 1.5f;
    private bool isDead = false;


    private Animator animator;

    private void Start()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("PlayerHealth requires an Animator component!");
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        //playerCollider.offset = new Vector2(0, 0.0853917f);
        //playerCollider.size = new Vector2(0.875f, 1.079217f);

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Player died!");

        StartCoroutine(RespawnSequence());
    }

    private IEnumerator RespawnSequence()
    {
        //playerCollider.offset = new Vector2(0, 0);
        //playerCollider.size = new Vector2(0.875f, 1.25f);

        yield return new WaitForSeconds(respawnDelay);

        if (CheckpointSystem.Instance != null)
        {
            CheckpointSystem.Instance.RespawnPlayer(gameObject);
        }
        animator.SetTrigger("Respawn");

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        isDead = false;

        Debug.Log("Player respawned!");
    }
}