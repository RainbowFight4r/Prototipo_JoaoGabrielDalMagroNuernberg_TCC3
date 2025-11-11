using UnityEngine;
using DG.Tweening;

public class LanternUpgradeItem : MonoBehaviour
{
    [Header("Visual Effects")]
    public float bobHeight = 0.5f;
    public float bobDuration = 1.5f;
    public PlayerLantern playerLantern;


    void Start()
    {
        StartFloatingAnimation();
    }

    void StartFloatingAnimation()
    {
        transform.DOMoveY(transform.position.y + bobHeight, bobDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerLantern.CollectUpgrade();
            Destroy(gameObject);

            Debug.Log("pegou o upgrade");

        }
    }

    void OnDestroy()
    {
        transform.DOKill();
    }
}