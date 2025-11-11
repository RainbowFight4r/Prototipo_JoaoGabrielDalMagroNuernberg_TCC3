using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    [SerializeField] private int coins = 0;
    [SerializeField] private int challengesCompleted = 0;

    [Header("UI References")]
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text challengesText;
    [SerializeField] private GameObject pnl_initial;

    [Header("Events")]
    public UnityEvent<int> OnCoinsChanged;
    public UnityEvent<int> OnChallengesChanged;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize UI
        UpdateCoinsUI();
        UpdateChallengesUI();
    }

    private void Start()
    {
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        pnl_initial.SetActive(false);
    }

    public void restartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    #region Coins Management
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        coins += amount;
        UpdateCoinsUI();
        OnCoinsChanged?.Invoke(coins);

        Debug.Log($"Coins added: {amount}. Total: {coins}");
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || coins < amount) return false;

        coins -= amount;
        UpdateCoinsUI();
        OnCoinsChanged?.Invoke(coins);

        Debug.Log($"Coins spent: {amount}. Remaining: {coins}");
        return true;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void SetCoins(int newAmount)
    {
        coins = Mathf.Max(0, newAmount);
        UpdateCoinsUI();
        OnCoinsChanged?.Invoke(coins);
    }
    #endregion

    #region Challenges Management
    public void AddChallengeCompleted()
    {
        challengesCompleted++;
        UpdateChallengesUI();
        OnChallengesChanged?.Invoke(challengesCompleted);

        Debug.Log($"Challenge completed! Total: {challengesCompleted}");
    }

    public int GetChallengesCompleted()
    {
        return challengesCompleted;
    }

    public void SetChallengesCompleted(int newAmount)
    {
        challengesCompleted = Mathf.Max(0, newAmount);
        UpdateChallengesUI();
        OnChallengesChanged?.Invoke(challengesCompleted);

        
    }
    #endregion

    #region UI Updates
    private void UpdateCoinsUI()
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }

    private void UpdateChallengesUI()
    {
        if (challengesText != null)
            challengesText.text = challengesCompleted.ToString();
    }

    public void SetCoinsUI(TMP_Text newCoinsText)
    {
        coinsText = newCoinsText;
        UpdateCoinsUI();
    }

    public void SetChallengesUI(TMP_Text newChallengesText)
    {
        challengesText = newChallengesText;
        UpdateChallengesUI();
    }
    #endregion

    #region Save/Load (Optional - for future use)
    [System.Serializable]
    public class GameData
    {
        public int coins;
        public int challengesCompleted;
    }

    public GameData GetGameData()
    {
        return new GameData
        {
            coins = this.coins,
            challengesCompleted = this.challengesCompleted
        };
    }

    public void LoadGameData(GameData data)
    {
        SetCoins(data.coins);
        SetChallengesCompleted(data.challengesCompleted);
    }
    #endregion
}