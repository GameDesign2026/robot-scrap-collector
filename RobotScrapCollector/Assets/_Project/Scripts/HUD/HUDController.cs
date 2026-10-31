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

    [Header("Renkler")]
    [SerializeField] private Color highColor = new Color(0.24f, 0.86f, 0.52f);   // yeşil (%50 üstü)
    [SerializeField] private Color midColor = new Color(1f, 0.84f, 0.31f);       // sarı (%25–50)
    [SerializeField] private Color lowColor = new Color(0.9f, 0.22f, 0.21f);     // kırmızı (%25 altı)

    [Header("Uyarı")]
    [SerializeField] private float warningLevel = 0.25f;   // bunun altında yanıp sön
    [SerializeField] private float blinkInterval = 0.3f;   // kaç saniyede bir

    private float timer;
    private bool blinkOn;

    void Start()
    {
        // Slotlar boşsa "Player" tag'li robotu bul
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

        // 1) Çubuğun doluluğu
        batteryFill.fillAmount = percent;

        // 2) Sayı: "72 / 100" (küsuratsız)
        if (batteryText != null)
        {
            int current = Mathf.CeilToInt(battery.GetCurrent());
            int max = Mathf.RoundToInt(battery.GetMax());
            batteryText.text = current + " / " + max;
        }

        // 3) Renk
        if (battery.IsEmpty())
        {
            batteryFill.color = lowColor;   // bitti: sürekli kırmızı
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

        // %25'in altı: kırmızı ile koyu kırmızı arasında yanıp sön
        timer += Time.deltaTime;
        if (timer >= blinkInterval)
        {
            timer = 0f;
            blinkOn = !blinkOn;
        }

        if (blinkOn)
        {
            batteryFill.color = lowColor;
        }
        else
        {
            batteryFill.color = lowColor * 0.4f;   // aynı renk, %40 parlaklık
        }
    }

    private void UpdateScrap()
    {
        if (storage == null || scrapText == null) return;
        scrapText.text = "Scrap: " + storage.GetScrap();
    }
}