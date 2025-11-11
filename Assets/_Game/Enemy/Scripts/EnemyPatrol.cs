using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movimentação")]
    public float speed = 2f;
    public bool flipSprite = true;

    [Header("Patrulha")]
    public Transform[] patrolPoints;
    public float waitTime = 2f;
    private int currentPatrolIndex = 0;

    [Header("Detecção de chão")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private bool isPatrolling = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning("Nenhum ponto de patrulha definido para " + gameObject.name);
            return;
        }

        transform.position = patrolPoints[0].position;

        StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        isPatrolling = true;

        while (isPatrolling)
        {
            Transform targetPoint = patrolPoints[currentPatrolIndex];

            Vector2 direction = targetPoint.position - transform.position;

            if ((direction.x > 0 && !facingRight) || (direction.x < 0 && facingRight))
            {
                Flip();
            }

            while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                direction = targetPoint.position - transform.position;
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPoint.position,
                    speed * Time.deltaTime
                );

                yield return null;
            }

            yield return new WaitForSeconds(waitTime);

            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        if (flipSprite)
        {
            spriteRenderer.flipX = !facingRight;

            EnemyDetection detection = GetComponent<EnemyDetection>();
            if (detection != null && detection.visionOrigin != null)
            {
                detection.FlipVisionDirection(!facingRight);
            }
        }
        else
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;

        }
    }

    void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        Gizmos.color = Color.red;
        foreach (Transform point in patrolPoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, 0.2f);
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] != null)
            {
                int nextIndex = (i + 1) % patrolPoints.Length;
                if (patrolPoints[nextIndex] != null)
                {
                    Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                }
            }
        }
    }

    public void StopPatrol()
    {
        isPatrolling = false;
        StopAllCoroutines();
    }

    public void ResumePatrol()
    {
        if (!isPatrolling)
        {
            StartCoroutine(Patrol());
        }
    }
}