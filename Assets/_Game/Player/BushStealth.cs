using UnityEngine;
using UnityEngine.Tilemaps;

public class BushStealth : MonoBehaviour
{
    public Tilemap bushTilemap;         
    public float transparentAlpha = 0.4f;
    public float normalAlpha = 1.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetTilemapAlpha(transparentAlpha);
            PlayerMovement    playerMovement = other.GetComponent<PlayerMovement>();
            CapsuleCollider2D playerCollider = other.GetComponent<CapsuleCollider2D>();

            if (playerMovement != null)
            {
                playerMovement.animator.SetTrigger("EnterStealth");
                playerMovement.animator.SetBool("InBush", true);

                playerCollider.offset = new Vector2(0, 0.0853917f);
                playerCollider.size = new Vector2(0.875f, 1.079217f);

                playerMovement.isInBush = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetTilemapAlpha(normalAlpha);
            PlayerMovement    playerMovement = other.GetComponent<PlayerMovement>();
            CapsuleCollider2D playerCollider = other.GetComponent<CapsuleCollider2D>();

            if (playerMovement != null)
            {
                playerMovement.animator.SetTrigger("ExitStealth");
                playerMovement.animator.SetBool("InBush", false);

                playerCollider.offset = new Vector2(0, 0);
                playerCollider.size = new Vector2(0.875f, 1.25f);

                playerMovement.isInBush = false;
            }
        }
    }

    private void SetTilemapAlpha(float alpha)
    {
        Color color = bushTilemap.color;
        color.a = alpha;
        bushTilemap.color = color;
    }
}
