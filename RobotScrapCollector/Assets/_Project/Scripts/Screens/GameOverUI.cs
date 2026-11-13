using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// SAHİBİ: Esra — 3 can bitince açılan ekran
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;      // GameOver'ın İÇİNDEKİ Panel
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string menuSceneName = "MainMenu";

    [SerializeField] private Color scrapColor = new Color(1f, 0.84f, 0.31f);   // sarı
    [SerializeField] private Color debtColor = new Color(1f, 0.32f, 0.32f);    // kırmızı (borçla bittiyse)

    void Awake()
    {
        panel.SetActive(false);
    }

    void Start()
    {
        retryButton.onClick.AddListener(Retry);
        menuButton.onClick.AddListener(GoToMenu);
    }

    // Ali'nin LivesManager'ı çağırır
    public void Show(int day, int scrap)
    {
        dayText.text = day + ". güne kadar dayandın";

        scrapText.text = "Toplam Scrap: " + scrap;
        scrapText.color = (scrap < 0) ? debtColor : scrapColor;

        panel.SetActive(true);

        // Butonlara tıklanabilsin
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool IsOpen()
    {
        return panel.activeSelf;
    }

    private void Retry()
    {
        Time.timeScale = 1f;   // ÖNCE zamanı geri aç (Ali 0 yaptı)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}