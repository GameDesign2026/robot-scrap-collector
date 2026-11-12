using UnityEngine;
using UnityEngine.UI;
using TMPro;

// SAHİBİ: Ali — ekranda batarya, Scrap, saat ve bahçedeki çöp
public class HUDController : MonoBehaviour
{
    [Header("Robot (boş bırakılırsa kendisi bulur)")]
    [SerializeField] private RobotBattery battery;
    [SerializeField] private RobotStorage storage;

    [Header("Ekran parçaları")]
    [SerializeField] private Image batteryFill;
    [SerializeField] private TMP_Text batteryText;
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private TMP_Text chargingText;
    [SerializeField] private TMP_Text clockText;      // YENİ
    [SerializeField] private TMP_Text gardenText;     // YENİ
    [SerializeField] private TMP_Text cleanupText;    // YENİ

    [Header("Renkler")]
    [SerializeField] private Color highColor = new Color(0.24f, 0.86f, 0.52f);
    [SerializeField] private Color midColor = new Color(1f, 0.84f, 0.31f);
    [SerializeField] private Color lowColor = new Color(0.9f, 0.22f, 0.21f);
    [SerializeField] private Color chargeColor = new Color(0.31f, 0.76f, 0.97f);
    [SerializeField] private Color cleanColor = new Color(0.24f, 0.86f, 0.52f);   // YENİ: bahçe temiz

    [Header("Uyarı")]
    [SerializeField] private float warningLevel = 0.25f;
    [SerializeField] private float blinkInterval = 0.3f;

    private DayClock clock;   // YENİ
    private float timer;
    private bool blinkOn;

    void Start()
    {
        if (battery == null || storage == null)
        {
            GameObject robot = GameObject.FindWithTag("Player");
            if (robot != null)
            {
                if (battery == null) battery = robot.GetComponent<RobotBattery>();
                if (storage == null) storage = robot.GetComponent<RobotStorage>();
            }
        }

        clock = FindFirstObjectByType<DayClock>();
    }

    void Update()
    {
        UpdateBattery();
        UpdateScrap();
        UpdateDay();   // YENİ
    }

    // ----- YENİ: saat, bahçe, uyarı -----
    private void UpdateDay()
    {
        // Saat
        if (clockText != null)
        {
            if (clock != null) clockText.text = "Gün " + clock.GetDay() + " · " + clock.GetTimeText();
            else clockText.text = "";
        }

        // Bahçedeki çöp
        if (gardenText != null)
        {
            int count = 0;
            if (GardenTracker.Instance != null) count = GardenTracker.Instance.GetCount();

            gardenText.text = "Bahçede: " + count + " çöp";
            gardenText.color = (count == 0) ? cleanColor : Color.white;
        }

        // Son saat uyarısı: nabız gibi atan kırmızı yazı
        if (cleanupText != null)
        {
            bool show = clock != null && clock.IsCleanupTime();
            cleanupText.gameObject.SetActive(show);

            if (show)
            {
                Color c = cleanupText.color;
                c.a = 0.55f + Mathf.Sin(Time.time * 5f) * 0.45f;   // 0.1 ile 1 arası
                cleanupText.color = c;
            }
        }
    }

    // ----- Aşağısı Aşama 1 ile aynı -----
    private void UpdateBattery()
    {
        if (battery == null || batteryFill == null) return;

        float percent = battery.GetPercent();
        batteryFill.fillAmount = percent;

        if (batteryText != null)
        {
            int current = Mathf.CeilToInt(battery.GetCurrent());
            int max = Mathf.RoundToInt(battery.GetMax());
            batteryText.text = current + " / " + max;
        }

        bool isCharging = battery.IsCharging();
        if (chargingText != null) chargingText.gameObject.SetActive(isCharging);

        if (isCharging)
        {
            float pulse = 0.85f + Mathf.Sin(Time.time * 6f) * 0.15f;
            batteryFill.color = chargeColor * pulse;
            timer = 0f;
            blinkOn = false;
            return;
        }

        if (battery.IsEmpty())
        {
            batteryFill.color = lowColor;
            return;
        }

        if (percent >= 0.5f)
        {
            batteryFill.color = highColor;
            timer = 0f;
            blinkOn = false;
            return;
        }

        if (percent >= warningLevel)
        {
            batteryFill.color = midColor;
            timer = 0f;
            blinkOn = false;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= blinkInterval)
        {
            timer = 0f;
            blinkOn = !blinkOn;
        }

        if (blinkOn) batteryFill.color = lowColor;
        else batteryFill.color = lowColor * 0.4f;
    }

    private void UpdateScrap()
    {
        if (storage == null || scrapText == null) return;
        scrapText.text = "Scrap: " + storage.GetScrap();
    }
}