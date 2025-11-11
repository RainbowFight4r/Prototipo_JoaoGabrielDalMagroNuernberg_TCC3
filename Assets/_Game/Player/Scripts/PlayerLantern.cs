using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerLantern : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private MobileJoystick _mJoystickR;

    [Header("Lantern Components")]
    public Light2D lanternLight;
    public Transform playerTransform;

    [Header("Upgraded Lantern Settings")]
    public float upgradedLightRange = 8f;
    public Color upgradedLightColor = Color.white;
    public float upgradedIntensity = 1f;

    [Header("Basic Lantern Settings")]
    public float basicLightRange = 3f;
    public Color basicLightColor = Color.yellow;
    public float basicIntensity = 0.6f;

    [Header("Malfunction Settings")]
    public float minMalfunctionTime = 2f;
    public float maxMalfunctionTime = 8f;
    public float malfunctionDuration = 0.5f;

    [Header("Status")]
    public bool hasUpgradedLantern = false;

    private Coroutine malfunctionCoroutine;
    private bool isMalfunctioning = false;
    private float originalIntensity;

    void Start()
    {
        SetBasicLantern();

        if (!hasUpgradedLantern)
        {
            StartCoroutine(MalfunctionRoutine());
        }
    }

    void Update()
    {
        LanternAngleUpdate();
    }

    void LanternAngleUpdate()
    {
        lanternLight.transform.position = playerTransform.position;

        Quaternion currentLanternRotation = lanternLight.transform.rotation;

#if UNITY_ANDROID
        float horizontal = _mJoystickR.InputDirection.x;
        float vertical = _mJoystickR.InputDirection.y;

        if (horizontal != 0f || vertical != 0f)
        {
            float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
            lanternLight.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
#else
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 direction = mousePosition - playerTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lanternLight.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
#endif

        if (hasUpgradedLantern)
        {
            lanternLight.pointLightOuterRadius = upgradedLightRange;
        }
        else
        {
            lanternLight.pointLightOuterRadius = basicLightRange;
        }
    }

    public void UpgradeLantern()
    {
        hasUpgradedLantern = true;
        SetUpgradedLantern();
        StopMalfunctionSystem();
    }

    void SetUpgradedLantern()
    {
        lanternLight.color = upgradedLightColor;
        lanternLight.intensity = upgradedIntensity;
        lanternLight.pointLightOuterRadius = upgradedLightRange;
    }

    void SetBasicLantern()
    {
        lanternLight.color = basicLightColor;
        lanternLight.intensity = basicIntensity;
        lanternLight.pointLightOuterRadius = basicLightRange;
        originalIntensity = basicIntensity;
    }


    void StopMalfunctionSystem()
    {
        if (malfunctionCoroutine != null)
        {
            StopCoroutine(malfunctionCoroutine);
            malfunctionCoroutine = null;
        }

        if (isMalfunctioning)
        {
            lanternLight.intensity = hasUpgradedLantern ? upgradedIntensity : basicIntensity;
            isMalfunctioning = false;
        }
    }

    IEnumerator MalfunctionRoutine()
    {
        while (!hasUpgradedLantern)
        {
            float waitTime = Random.Range(minMalfunctionTime, maxMalfunctionTime);
            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(ExecuteMalfunction());
        }
    }

    IEnumerator ExecuteMalfunction()
    {
        isMalfunctioning = true;

        lanternLight.intensity = 0.1f;

        yield return new WaitForSeconds(malfunctionDuration);

        lanternLight.intensity = originalIntensity;
        isMalfunctioning = false;
    }

    public void CollectUpgrade()
    {
        UpgradeLantern();
    }
}