using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// SAHİBİ: Esra — Esc ile oyunu duraklat
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;      // PauseMenu'nün İÇİNDEKİ Panel
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string menuSceneName = "MainMenu";

    private bool paused;
    private bool cursorWasLocked;   // duraklatmadan önce imleç kilitli miydi?

    private MarketUI market;
    private DaySummaryUI summary;
    private GameOverUI gameOver;

    void Awake()
    {
        panel.SetActive(false);
    }

    void Start()
    {
        resumeButton.onClick.AddListener(Resume);
        menuButton.onClick.AddListener(GoToMenu);

        // Diğer ekranları bul (gizli olsalar bile)
        market = FindFirstObjectByType<MarketUI>(FindObjectsInactive.Include);
        summary = FindFirstObjectByType<DaySummaryUI>(FindObjectsInactive.Include);
        gameOver = FindFirstObjectByType<GameOverUI>(FindObjectsInactive.Include);
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;

        if (paused) Resume();
        else if (CanPause()) Pause();
    }

    // Başka bir ekran açıkken duraklatma
    private bool CanPause()
    {
        if (market != null && market.IsOpen()) return false;
        if (summary != null && summary.IsOpen()) return false;
        if (gameOver != null && gameOver.IsOpen()) return false;
        return true;
    }

    private void Pause()
    {
        paused = true;
        cursorWasLocked = Cursor.lockState == CursorLockMode.Locked;

        Time.timeScale = 0f;   // her şey durur
        panel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        panel.SetActive(false);

        // Duraklatmadan önce kilitliyse tekrar kilitle
        if (cursorWasLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}