using UnityEngine;

public class CollectionDialogueTrigger : MonoBehaviour
{
    [SerializeField] CollectionDialogueManager dialogueManager;
    [SerializeField] bool triggerOnce = true;

    bool hasTriggered = false;

    void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<CollectionDialogueManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue();
                hasTriggered = true;

                if (triggerOnce)
                    GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}