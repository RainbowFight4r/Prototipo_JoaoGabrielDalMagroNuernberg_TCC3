using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionDialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text npcNameText;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] Button proceedButton;

    [Header("Minigame")]
    [SerializeField] CollectionMinigame collectionMinigame;

    [Header("Dialogue")]
    [SerializeField] string npcName = "Professor Esqueleto";
    [TextArea(3, 10)]
    [SerializeField]
    string dialogueContent =
        "Olá! Bem-vindo ao Desafio Estatístico!\n\n" +
        "Neste teste, você precisará coletar blocos coloridos espalhados pelo mapa. " +
        "Use seu dash e double jump para alcançar todos os lugares!\n\n" +
        "Ao final, vou analisar sua coleta usando conceitos estatísticos como distribuição de frequências, " +
        "medidas de tendência central e dispersão.\n\n" +
        "Está pronto para o desafio?";

    [SerializeField, Range(0.005f, 0.05f)] float charDelay = 0.02f;

    Coroutine typingCo;

    void Awake()
    {
        if (dialoguePanel)
            dialoguePanel.SetActive(false);

        if (proceedButton != null)
        {
            proceedButton.onClick.RemoveAllListeners();
            proceedButton.onClick.AddListener(OnProceedPressed);
            proceedButton.gameObject.SetActive(false);
        }
    }

    public void StartDialogue()
    {
        if (!dialoguePanel) return;

        dialoguePanel.SetActive(true);

        if (npcNameText)
            npcNameText.text = npcName;

        if (typingCo != null)
            StopCoroutine(typingCo);

        typingCo = StartCoroutine(TypeLine(dialogueContent));
    }

    IEnumerator TypeLine(string line)
    {
        proceedButton?.gameObject.SetActive(false);
        if (dialogueText) dialogueText.text = "";

        foreach (char c in line)
        {
            if (dialogueText != null)
                dialogueText.text += c;

            yield return new WaitForSeconds(charDelay);
        }

        proceedButton?.gameObject.SetActive(true);
    }

    public void OnProceedPressed()
    {
        if (dialoguePanel)
            dialoguePanel.SetActive(false);

        if (collectionMinigame != null)
            collectionMinigame.StartGame();
        else
            Debug.LogError("CollectionMinigame não está referenciado!");
    }
}