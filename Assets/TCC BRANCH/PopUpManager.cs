using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [Header("PopUp Prefab")]
    [SerializeField] GameObject popUpPrefab; // Prefab com CanvasGroup + TMP_Text
    [SerializeField] Transform popUpParent; // Canvas onde vai spawnar

    [Header("Configurações")]
    [SerializeField] float fadeInDuration = 0.5f;
    [SerializeField] float displayDuration = 3f;
    [SerializeField] float fadeOutDuration = 0.5f;
    [SerializeField] Vector3 popUpScale = new Vector3(0.8f, 0.8f, 1f);

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowPopUp(string message)
    {
        if (popUpPrefab == null)
        {
            Debug.LogError("PopUp Prefab não atribuído!");
            return;
        }

        GameObject popUp = Instantiate(popUpPrefab, popUpParent);
        TMP_Text text = popUp.GetComponentInChildren<TMP_Text>();
        CanvasGroup canvasGroup = popUp.GetComponent<CanvasGroup>();

        if (text != null)
            text.text = message;

        if (canvasGroup == null)
            canvasGroup = popUp.AddComponent<CanvasGroup>();

        // Setup inicial
        canvasGroup.alpha = 0;
        popUp.transform.localScale = popUpScale;

        // Animação
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1f, fadeInDuration));
        sequence.AppendInterval(displayDuration);
        sequence.Append(canvasGroup.DOFade(0f, fadeOutDuration));
        sequence.OnComplete(() => Destroy(popUp));
    }

    public void ShowDoubleJumpUnlocked()
    {
        ShowPopUp("Você desbloqueou o pulo duplo!\nPressione espaço 2 vezes");
    }

    public void ShowPathUnlocked()
    {
        ShowPopUp("Você liberou um caminho!");
    }
}