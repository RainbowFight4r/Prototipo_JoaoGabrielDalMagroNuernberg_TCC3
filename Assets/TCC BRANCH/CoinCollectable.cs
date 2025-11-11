// CoinCollectible.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinCollectible : MonoBehaviour
{
    [SerializeField] int value = 1;
    [SerializeField] AudioClip sfx;
    [SerializeField] GameObject pickupVfx;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Usa o GameManager ao invés do SimplePlayerStats
        if (GameManager.Instance != null)
            GameManager.Instance.AddCoins(value);
        else
            Debug.LogWarning("GameManager não encontrado! Certifique-se de que existe um GameManager na cena.");

        // Efeitos visuais e sonoros
        if (pickupVfx)
            Instantiate(pickupVfx, transform.position, Quaternion.identity);

        if (sfx)
            AudioSource.PlayClipAtPoint(sfx, transform.position);

        Destroy(gameObject);
    }
}