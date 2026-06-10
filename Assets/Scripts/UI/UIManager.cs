using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Score")]
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI highScoreText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!pauseScreen.activeInHierarchy);
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (DinoGameManager.instance == null) return;

        if (scoreText != null)
        {
            scoreText.text = Mathf.FloorToInt(DinoGameManager.instance.score).ToString();
        }
        if (highScoreText != null)
        {
            highScoreText.text = "HI " + Mathf.FloorToInt(DinoGameManager.instance.highScore).ToString();
        }
    }

    #region Game Over
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        if (DinoGameManager.instance != null)
            DinoGameManager.instance.Restart();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        pauseScreen.SetActive(status);
        if (status)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }
    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
    #endregion
}