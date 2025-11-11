using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text npcNameText;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] Button proceedButton;

    [Header("Minigame")]
    [SerializeField] GameObject minigamePanel;

    [Header("Fal falas possíveis")]
    [TextArea(3, 5)]
    [SerializeField]
    string falaPadrao =
        "Olá! Eu sou o Professor. Hoje vamos revisar tipos de variáveis. " +
        "Quando estiver pronto, pressione Prosseguir para começar o quiz.";

    [TextArea(3, 5)]
    [SerializeField]
    string falaAlternativa =
        "Olá novamente! Agora vamos para um desafio diferente. " +
        "Mostre que você lembra do que já aprendeu.";

    [Header("Typing Effect")]
    [SerializeField, Range(0.005f, 0.05f)] float charDelay = 0.02f;
    Coroutine typingCo;

    SimpleVariableGame quizAtual;

    void Awake()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        if (minigamePanel) minigamePanel.SetActive(false);

        if (proceedButton != null)
        {
            proceedButton.onClick.RemoveAllListeners();
            proceedButton.onClick.AddListener(OnProceedPressed);
            proceedButton.gameObject.SetActive(false);
        }
    }

    public void StartDialogue(SimpleVariableGame quizRef, bool usarAlternativa)
    {
        if (!dialoguePanel) return;

        quizAtual = quizRef;

        dialoguePanel.SetActive(true);
        if (npcNameText) npcNameText.text = "Professor";

        string fala = usarAlternativa ? falaAlternativa : falaPadrao;

        if (typingCo != null) StopCoroutine(typingCo);
        typingCo = StartCoroutine(TypeLine(fala));
    }

    IEnumerator TypeLine(string line)
    {
        proceedButton?.gameObject.SetActive(false);
        if (dialogueText) dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        proceedButton?.gameObject.SetActive(true);
    }

    public void OnProceedPressed()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);

        if (minigamePanel) minigamePanel.SetActive(true);
        quizAtual?.StartGame();
    }
}
