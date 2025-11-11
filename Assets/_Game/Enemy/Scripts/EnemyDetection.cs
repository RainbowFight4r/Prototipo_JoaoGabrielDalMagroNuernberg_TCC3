using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [Header("Configurações de Detecção")]
    public float detectionRange = 5f;
    public float detectionAngle = 45f;
    public LayerMask playerLayer;

    [Header("Atributos do Inimigo")]
    public Transform visionOrigin;
    public Transform player;

    [Header("Player Death Settings")]
    public bool killPlayerOnDetection = true;

    private bool playerDetected = false;
    private bool facingLeft = false;
    private float detectionTimer = 0f;
    private bool killSequenceStarted = false;

    void Update()
    {
        DetectPlayer();

        if (GetComponent<EnemyPatrol>()?.flipSprite == false)
        {
            AdjustConeDirection();
        }

        HandlePlayerDetection();
    }

    void DetectPlayer()
    {
        if (player == null) return;

        Vector2 directionToPlayer = player.position - visionOrigin.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange && PlayerMovement.instance.isInBush == false)
        {
            Vector2 forwardDirection = visionOrigin.right;

            float angleToPlayer = Vector2.Angle(forwardDirection, directionToPlayer);

            if (angleToPlayer <= detectionAngle / 2f)
            {
                RaycastHit2D hit = Physics2D.Raycast(visionOrigin.position, directionToPlayer, distanceToPlayer, playerLayer);

                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    playerDetected = true;
                    Debug.Log("Jogador Detectado!");
                }
                else
                {
                    playerDetected = false;
                    ResetDetectionSequence();
                }
            }
            else
            {
                playerDetected = false;
                ResetDetectionSequence();
            }
        }
        else
        {
            playerDetected = false;
            ResetDetectionSequence();
        }
    }

    void HandlePlayerDetection()
    {
        if (playerDetected && killPlayerOnDetection && !killSequenceStarted)
        {
            detectionTimer += Time.deltaTime;
            KillPlayer();
            killSequenceStarted = true;
        }
    }

    void KillPlayer()
    {
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Die();
                Debug.Log("Enemy killed player!");
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on player!");
            }
        }
    }

    void ResetDetectionSequence()
    {
        detectionTimer = 0f;
        killSequenceStarted = false;
    }

    void AdjustConeDirection()
    {
        float rotationZ = transform.localScale.x < 0 ? 180f : 0f;
        visionOrigin.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    public void FlipVisionDirection(bool isFlipped)
    {
        facingLeft = isFlipped;
        float rotationZ = facingLeft ? 180f : 0f;
        visionOrigin.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    void OnDrawGizmosSelected()
    {
        if (visionOrigin == null) return;

        Vector3 forward = visionOrigin.right * detectionRange;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(visionOrigin.position, visionOrigin.position + forward);

        Gizmos.color = playerDetected ? Color.yellow : Color.red;
        float halfAngle = detectionAngle / 2f;

        float step = 5f;
        for (float i = -halfAngle; i <= halfAngle; i += step)
        {
            Vector3 direction = Quaternion.Euler(0, 0, i) * forward;
            Gizmos.DrawLine(visionOrigin.position, visionOrigin.position + direction);
        }

        if (player != null)
        {
            Gizmos.color = playerDetected ? Color.green : Color.blue;
            Gizmos.DrawLine(visionOrigin.position, player.position);
        }
    }

    public bool IsPlayerDetected()
    {
        return playerDetected;
    }
}