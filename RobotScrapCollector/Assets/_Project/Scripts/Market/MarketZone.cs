using UnityEngine;
using UnityEngine.InputSystem;

// SAHİBİ: Esra — market bölgesi: teslim, şarj ve E ile alışveriş
public class MarketZone : MonoBehaviour
{
    private MarketUI marketUI;          // sahnedeki market paneli
    private InputAction interactAction; // E tuşu
    private GameObject robotInside;     // içerideki robot (dışarıdaysa null)

    void Start()
    {
        marketUI = FindFirstObjectByType<MarketUI>();
        if (marketUI == null)
        {
            Debug.LogWarning("MarketZone: sahnede MarketUI yok, panel açılamaz");
        }

        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        // Robot içeride değilse ya da panel yoksa hiçbir şey yapma
        if (robotInside == null || marketUI == null) return;

        // E'ye basıldı mı?
        if (interactAction == null || !interactAction.WasPressedThisFrame()) return;

        if (marketUI.IsOpen())
        {
            marketUI.Close();
        }
        else
        {
            Unload(robotInside);           // içeride yerden alınmış bir çöp varsa onu da teslim et
            marketUI.Open(robotInside);
        }
    }

    // Robot markete GİRDİ
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        robotInside = other.gameObject;

        // 1) Taşıdığı çöpleri teslim et
        Unload(robotInside);

        // 2) Şarjı başlat
        RobotBattery battery = robotInside.GetComponent<RobotBattery>();
        if (battery != null) battery.SetCharging(true);

        // 3) "E – Market" yazısını göster
        if (marketUI != null) marketUI.SetPromptVisible(true);
    }

    // Robot marketten ÇIKTI
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        RobotBattery battery = other.GetComponent<RobotBattery>();
        if (battery != null) battery.SetCharging(false);

        if (marketUI != null) marketUI.SetPromptVisible(false);

        robotInside = null;
    }

    private void Unload(GameObject robot)
    {
        RobotStorage storage = robot.GetComponent<RobotStorage>();
        if (storage != null) storage.Unload();
    }
}