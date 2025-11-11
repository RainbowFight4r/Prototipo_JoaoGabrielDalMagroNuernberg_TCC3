using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleVariableGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] Button nextButton;
    [SerializeField] GameObject gamePanel;

    [Header("Answers (Prefab)")]
    [SerializeField] GameObject answerItemPrefab;
    [SerializeField] Transform answersParent;

    [Header("Rewards")]
    [SerializeField] int coinsPerCorrect = 1;
    [SerializeField] int coinsOnComplete = 0;

    [Header("Quiz Settings")]
    [SerializeField] int quizID = 1; // ID do quiz (1 = double jump, 2 = ativa objeto)

    [Header("Recompensa do Quiz")]
    [SerializeField] GameObject objectToActivate; // GameObject que será ativado no quiz 2

    [System.Serializable]
    public class Question
    {
        public string enunciado;
        public List<string> opcoes;
        public int indiceCorreto;
    }

    [SerializeField] List<Question> perguntas = new List<Question>();

    int indexAtual = 0;
    int acertos = 0;
    bool respondeu = false;

    public void StartGame()
    {
        indexAtual = 0;
        acertos = 0;
        respondeu = false;

        if (scoreText) scoreText.text = "0 acertos";
        if (feedbackText) feedbackText.text = "";

        if (nextButton)
        {
            nextButton.gameObject.SetActive(true);
            nextButton.interactable = false;
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextQuestion);
        }

        MostrarPerguntaAtual();
    }

    void LimparRespostas()
    {
        if (!answersParent) return;
        for (int i = answersParent.childCount - 1; i >= 0; i--)
            Destroy(answersParent.GetChild(i).gameObject);
    }

    void MostrarPerguntaAtual()
    {
        respondeu = false;
        if (nextButton) nextButton.interactable = false;
        if (feedbackText) feedbackText.text = "";

        var q = perguntas[indexAtual];
        if (questionText) questionText.text = q.enunciado;

        LimparRespostas();

        for (int i = 0; i < q.opcoes.Count; i++)
        {
            GameObject go = Instantiate(answerItemPrefab, answersParent);

            Button btn = go.GetComponent<Button>();
            if (btn == null) btn = go.GetComponentInChildren<Button>(true);

            TMP_Text label = go.GetComponent<TMP_Text>();
            if (label == null) label = go.GetComponentInChildren<TMP_Text>(true);

            if (label != null) label.text = q.opcoes[i];

            if (btn != null)
            {
                int captured = i;
                btn.onClick.RemoveAllListeners();
                btn.interactable = true;
                btn.onClick.AddListener(() => AnswerSelected(captured));
            }
        }
    }

    public void AnswerSelected(int idx)
    {
        if (respondeu) return;
        respondeu = true;

        var q = perguntas[indexAtual];
        bool correta = (idx == q.indiceCorreto);

        if (correta)
        {
            acertos++;
            if (feedbackText) feedbackText.text = "Resposta correta!";
        }
        else
        {
            if (feedbackText) feedbackText.text =
                $"Incorreta. Correta: {q.opcoes[q.indiceCorreto]}";
        }

        if (scoreText) scoreText.text = $"{acertos} acerto(s)";
        if (nextButton) nextButton.interactable = true;

        if (answersParent)
        {
            foreach (Transform t in answersParent)
            {
                var b = t.GetComponentInChildren<Button>(true);
                if (b) b.interactable = false;
            }
        }
    }

    public void NextQuestion()
    {
        indexAtual++;
        if (indexAtual >= perguntas.Count)
        {
            FimDoQuiz();
            return;
        }
        MostrarPerguntaAtual();
    }

    void FimDoQuiz()
    {
        if (questionText) questionText.text = "Fim do quiz!";
        if (feedbackText) feedbackText.text = $"Você acertou {acertos} de {perguntas.Count}.";
        if (nextButton) nextButton.gameObject.SetActive(false);
        LimparRespostas();

        // Recompensa usando o GameManager
        int coinsEarned = acertos * coinsPerCorrect + coinsOnComplete;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(coinsEarned);
            GameManager.Instance.AddChallengeCompleted();
        }
        else
        {
            Debug.LogWarning("GameManager não encontrado! Certifique-se de que existe um GameManager na cena.");
        }

        // Libera habilidades dependendo do quiz
        if (PlayerMovement.instance != null)
        {
            if (quizID == 1)
            {
                PlayerMovement.instance.enableDoubleJump = true;
                Debug.Log("Double Jump liberado!");

                // Mostra PopUp
                if (PopUpManager.Instance != null)
                    PopUpManager.Instance.ShowDoubleJumpUnlocked();
            }
            else if (quizID == 2)
            {
                // Ativa o GameObject ao invés de dar dash
                if (objectToActivate != null)
                {
                    objectToActivate.SetActive(true);
                    Debug.Log($"GameObject '{objectToActivate.name}' foi ativado!");

                    // Mostra PopUp
                    if (PopUpManager.Instance != null)
                        PopUpManager.Instance.ShowPathUnlocked();
                }
                else
                {
                    Debug.LogWarning("Nenhum GameObject foi atribuído para ser ativado no quiz 2!");
                }
            }
        }

        StartCoroutine(CloseAfterDelay());
    }

    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (gamePanel) gamePanel.SetActive(false);
    }
}