using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class LanternFocus : MonoBehaviour
{
    [Header("Focus Settings")]
    [SerializeField] private float focusTime = 2f;
    [SerializeField] private float stunDuration = 5f;

    [Header("Camera Effects")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomAmount = 1.5f;
    [SerializeField] private float shakeIntensity = 0.3f;
    [SerializeField] private float shakeDuration = 0.1f;

    [Header("Light2D Focus Effects")]
    [SerializeField] private Light2D lanternLight;
    [SerializeField] private float focusedOuterRadius = 2f;
    [SerializeField] private float focusedIntensity = 5f;
    [SerializeField] private float minInnerSpotAngle = 10f;
    [SerializeField] private float minOuterSpotAngle = 20f;

    private PlayerLantern playerLantern;
    private bool isFocusing = false;
    private float focusProgress = 0f;
    private GameObject currentTarget = null;
    private EnemyDetection targetEnemy = null;

    // Original Light2D values
    private float originalCameraSize;
    private float originalOuterRadius;
    private float originalIntensity;
    private float originalInnerSpotAngle;
    private float originalOuterSpotAngle;

    // Tweens
    private Tween cameraTween;
    private Tween shakeTween;

    void Start()
    {
        playerLantern = GetComponent<PlayerLantern>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        // Store original Light2D values
        originalCameraSize = mainCamera.orthographicSize;
        originalOuterRadius = lanternLight.pointLightOuterRadius;
        originalIntensity = lanternLight.intensity;
        originalInnerSpotAngle = lanternLight.pointLightInnerAngle;
        originalOuterSpotAngle = lanternLight.pointLightOuterAngle;
    }

    void Update()
    {
        // Only works with upgraded lantern
        if (!playerLantern.hasUpgradedLantern)
            return;

        HandleFocusInput();

        if (isFocusing)
        {
            UpdateFocus();
        }
    }

    void HandleFocusInput()
    {
        bool focusing = Input.GetMouseButton(1); // Right mouse button

        if (focusing && !isFocusing)
        {
            StartFocus();
        }
        else if (!focusing && isFocusing)
        {
            StopFocus();
        }
    }

    void StartFocus()
    {
        isFocusing = true;
        focusProgress = 0f;

        // Find target enemy
        FindTargetEnemy();

        // Camera zoom and shake
        StartCameraEffects();

        // Lantern focus effects
        StartLanternFocus();
    }

    void StopFocus()
    {
        isFocusing = false;
        focusProgress = 0f;
        currentTarget = null;
        targetEnemy = null;

        // Stop camera effects
        StopCameraEffects();

        // Reset lantern effects
        ResetLanternEffects();
    }

    void FindTargetEnemy()
    {
        Vector3 lanternPosition = lanternLight.transform.position;
        Vector3 lanternDirection = lanternLight.transform.up; // Changed from right to up

        // Use actual light parameters
        float searchRadius = lanternLight.pointLightOuterRadius;
        float searchAngle = lanternLight.pointLightOuterAngle;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(lanternPosition, searchRadius);

        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Vector3 directionToEnemy = enemy.transform.position - lanternPosition;
                float angle = Vector3.Angle(lanternDirection, directionToEnemy);

                if (angle <= searchAngle / 2f)
                {
                    float distance = directionToEnemy.magnitude;
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy.gameObject;
                    }
                }
            }
        }

        currentTarget = closestEnemy;
        if (currentTarget != null)
        {
            targetEnemy = currentTarget.GetComponent<EnemyDetection>();
        }
    }

    void UpdateFocus()
    {
        if (currentTarget == null)
        {
            FindTargetEnemy();
            return;
        }

        // Check if still targeting using actual light parameters
        Vector3 lanternPosition = lanternLight.transform.position;
        Vector3 lanternDirection = lanternLight.transform.up; // Changed from right to up
        Vector3 directionToEnemy = currentTarget.transform.position - lanternPosition;
        float angle = Vector3.Angle(lanternDirection, directionToEnemy);
        float distance = directionToEnemy.magnitude;

        // Use current light cone for detection
        float currentAngle = lanternLight.pointLightOuterAngle;
        float currentRadius = lanternLight.pointLightOuterRadius;

        if (angle <= currentAngle / 2f && distance <= currentRadius)
        {
            // Increase focus progress
            focusProgress += Time.deltaTime;

            // Continuous camera shake while focusing
            if (shakeTween == null || !shakeTween.IsActive())
            {
                ShakeCamera();
            }

            // Check if enemy should be stunned
            if (focusProgress >= focusTime && targetEnemy != null)
            {
                StunEnemy();
            }
        }
        else
        {
            // Lost target, reset progress but don't stop focusing
            focusProgress = 0f;
            currentTarget = null;
            targetEnemy = null;
        }
    }

    void StartCameraEffects()
    {
        // Zoom in
        cameraTween = mainCamera.DOOrthoSize(originalCameraSize / zoomAmount, 0.3f)
            .SetEase(Ease.OutQuad);

        // Start shaking
        ShakeCamera();
    }

    void StopCameraEffects()
    {
        // Stop all camera tweens
        if (cameraTween != null)
            cameraTween.Kill();
        if (shakeTween != null)
            shakeTween.Kill();

        // Zoom out
        cameraTween = mainCamera.DOOrthoSize(originalCameraSize, 0.3f)
            .SetEase(Ease.OutQuad);

    }

    void ShakeCamera()
    {
        shakeTween = mainCamera.transform.DOShakePosition(shakeDuration, shakeIntensity, 10, 90f)
            .OnComplete(() => {
                if (isFocusing)
                {
                    // Continue shaking while focusing
                    ShakeCamera();
                }
            });
    }

    void StartLanternFocus()
    {
        DOTween.To(() => lanternLight.intensity, x => lanternLight.intensity = x, focusedIntensity, 0.3f);
        DOTween.To(() => lanternLight.pointLightInnerAngle, x => lanternLight.pointLightInnerAngle = x, minInnerSpotAngle, 0.3f);
        DOTween.To(() => lanternLight.pointLightOuterAngle, x => lanternLight.pointLightOuterAngle = x, minOuterSpotAngle, 0.3f);
    }

    void ResetLanternEffects()
    {
        // Return Light2D to original state
        float targetOuterRadius = playerLantern.hasUpgradedLantern ? playerLantern.upgradedLightRange : playerLantern.basicLightRange;
        float targetIntensity = playerLantern.hasUpgradedLantern ? playerLantern.upgradedIntensity : playerLantern.basicIntensity;

        DOTween.To(() => lanternLight.pointLightOuterRadius, x => lanternLight.pointLightOuterRadius = x, targetOuterRadius, 0.3f);
        DOTween.To(() => lanternLight.intensity, x => lanternLight.intensity = x, targetIntensity, 0.3f);
        DOTween.To(() => lanternLight.pointLightInnerAngle, x => lanternLight.pointLightInnerAngle = x, originalInnerSpotAngle, 0.3f);
        DOTween.To(() => lanternLight.pointLightOuterAngle, x => lanternLight.pointLightOuterAngle = x, originalOuterSpotAngle, 0.3f);
    }

    void StunEnemy()
    {
        if (targetEnemy != null)
        {
            // Get patrol component and stop it
            EnemyPatrol patrol = targetEnemy.GetComponent<EnemyPatrol>();
            if (patrol != null)
            {
                StartCoroutine(StunEnemyRoutine(patrol));
            }

            // Visual feedback
            StartCoroutine(StunVisualFeedback());

            // Reset focus
            focusProgress = 0f;

            Debug.Log("Enemy stunned!");
        }
    }

    IEnumerator StunEnemyRoutine(EnemyPatrol patrol)
    {
        // Stop enemy patrol
        patrol.StopPatrol();

        // Disable enemy detection
        if (targetEnemy != null)
        {
            targetEnemy.enabled = false;
        }

        // Wait for stun duration
        yield return new WaitForSeconds(stunDuration);

        // Resume enemy behavior
        patrol.ResumePatrol();
        if (targetEnemy != null)
        {
            targetEnemy.enabled = true;
        }

        Debug.Log("Enemy recovered from stun");
    }

    IEnumerator StunVisualFeedback()
    {
        // Flash the enemy sprite or add particle effect here
        SpriteRenderer enemySprite = currentTarget.GetComponent<SpriteRenderer>();
        if (enemySprite != null)
        {
            Color originalColor = enemySprite.color;

            // Flash effect
            for (int i = 0; i < 6; i++)
            {
                enemySprite.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                enemySprite.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (lanternLight == null) return;

        Vector3 lanternPosition = lanternLight.transform.position;
        Vector3 lanternDirection = lanternLight.transform.up; // Changed from right to up

        // Use actual light parameters for gizmos
        float drawRadius = lanternLight.pointLightOuterRadius;
        float drawAngle = lanternLight.pointLightOuterAngle;

        // Draw focus range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(lanternPosition, drawRadius);

        // Draw focus cone matching light
        Gizmos.color = isFocusing ? Color.yellow : Color.blue;
        float halfAngle = drawAngle / 2f;

        Vector3 leftBoundary = Quaternion.Euler(0, 0, halfAngle) * (lanternDirection * drawRadius);
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -halfAngle) * (lanternDirection * drawRadius);

        Gizmos.DrawLine(lanternPosition, lanternPosition + leftBoundary);
        Gizmos.DrawLine(lanternPosition, lanternPosition + rightBoundary);

        // Draw arc
        for (float i = -halfAngle; i <= halfAngle; i += 5f)
        {
            Vector3 direction = Quaternion.Euler(0, 0, i) * (lanternDirection * drawRadius);
            Gizmos.DrawLine(lanternPosition, lanternPosition + direction);
        }

        // Draw target
        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentTarget.transform.position, 0.5f);
        }
    }
}