using UnityEngine;
using UnityEngine.UI;
using TMPro;

// SAHİBİ: Ali — ekranda batarya ve Scrap gösterir
public class HUDController : MonoBehaviour
{
    [Header("Robot (boş bırakılırsa kendisi bulur)")]
    [SerializeField] private RobotBattery battery;
    [SerializeField] private RobotStorage storage;

    [Header("Ekran parçaları")]
    [SerializeField] private Image batteryFill;
    [SerializeField] private TMP_Text batteryText;
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private TMP_Text chargingText;   // YENİ: "ŞARJ OLUYOR"

    [Header("Renkler")]
    [SerializeField] private Color highColor = new Color(0.24f, 0.86f, 0.52f);
    [SerializeField] private Color midColor = new Color(1f, 0.84f, 0.31f);
    [SerializeField] private Color lowColor = new Color(0.9f, 0.22f, 0.21f);
    [SerializeField] private Color chargeColor = new Color(0.31f, 0.76f, 0.97f);   // YENİ: açık mavi

    [Header("Uyarı")]
    [SerializeField] private float warningLevel = 0.25f;
    [SerializeField] private float blinkInterval = 0.3f;

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
    }

    void Update()
    {
        UpdateBattery();
        UpdateScrap();
    }

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

        // YENİ: şarj oluyorsa açık mavi ve yazı görünür
        bool isCharging = battery.IsCharging();
        if (chargingText != null) chargingText.gameObject.SetActive(isCharging);

        if (isCharging)
        {
            // Hafifçe nabız gibi atsın: parlaklık 0.7 ile 1 arasında gidip gelir
            float pulse = 0.85f + Mathf.Sin(Time.time * 6f) * 0.15f;
            batteryFill.color = chargeColor * pulse;
            timer = 0f;
            blinkOn = false;
            return;
        }

        // Aşağısı 5. haftadakiyle aynı
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