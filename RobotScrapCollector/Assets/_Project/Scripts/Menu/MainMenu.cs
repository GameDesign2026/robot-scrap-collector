using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// SAHİBİ: Hamza — ana menü: Başla ve Çıkış
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string gameSceneName = "Main";

    void Start()
    {
        // Oyundan geri dönülünce: imleç görünsün, zaman aksın
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void QuitGame()
    {
        Debug.Log("Çıkış");   // editörde Quit çalışmaz, en azından görelim
        Application.Quit();
    }
}