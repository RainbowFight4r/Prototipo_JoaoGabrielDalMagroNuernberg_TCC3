using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] DialogueManager dm;
    [SerializeField] SimpleVariableGame quizAlvo;

    [Header("Controle de fala")]
    [SerializeField] bool usarFalaAlternativa = false;

    void Awake()
    {
        if (dm == null)
            dm = FindObjectOfType<DialogueManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && dm != null)
        {
            dm.StartDialogue(quizAlvo, usarFalaAlternativa); // passa o quiz e qual fala usar
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
