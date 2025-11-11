using UnityEngine;
using DG.Tweening;

public class CollectibleBlock : MonoBehaviour
{
    CollectionMinigame minigame;
    string blockType;
    bool collected = false;

    public void Initialize(CollectionMinigame game, string type)
    {
        minigame = game;
        blockType = type;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            if (minigame != null)
                minigame.CollectBlock(blockType);

            transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            Destroy(gameObject, 0.3f);
        }
    }

    void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}