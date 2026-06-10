using UnityEngine;

public class DinoGameManager : MonoBehaviour
{
    public static DinoGameManager instance { get; private set; }

    [Header("Game State")]
    public bool isPlaying = false;

    [Header("Speed")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float speedIncreasePerCheckpoint = 1.2f;
    [SerializeField] private float maxSpeedMultiplier = 3f;
    private float speedMultiplier = 1f;

    [Header("Score")]
    public float score { get; private set; }
    public float highScore { get; private set; }

    public float CurrentSpeed => baseSpeed * speedMultiplier;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        highScore = PlayerPrefs.GetFloat("HighScore", 0);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        isPlaying = true;
        score = 0;
        speedMultiplier = 1f;
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        isPlaying = false;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", score);
        }
        UIManager.instance?.GameOver();
    }

    public void AddScore(float amount)
    {
        if (!isPlaying) return;
        score += amount;
    }

    public void IncreaseSpeed()
    {
        speedMultiplier = Mathf.Min(speedMultiplier * speedIncreasePerCheckpoint, maxSpeedMultiplier);
    }

    public void Restart()
    {
        StartGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
