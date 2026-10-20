using UnityEngine;

// SAHİBİ: Nagihan
public class LowBatteryWarning : MonoBehaviour
{
    [SerializeField] private Renderer eyeRenderer;                        // robotun gözü (Inspector'dan sürükle)
    [SerializeField] private float warningLevel = 0.25f;                  // bu oranın altında uyar (0.25 = %25)
    [SerializeField] private float blinkInterval = 0.3f;                  // kaç saniyede bir yanıp sönsün
    [SerializeField] private Color warningColor = new Color(1f, 0.2f, 0.2f); // uyarı rengi: kırmızı

    private RobotBattery battery;
    private Color normalColor;   // gözün normal rengi (oyun başında kaydedilir)
    private float timer;         // yanıp sönme sayacı
    private bool redOn;          // şu an kırmızı mı?

    void Awake()
    {
        battery = GetComponent<RobotBattery>();
    }

    void Start()
    {
        // Gözün normal rengini hatırla (mavi)
        if (eyeRenderer != null)
        {
            normalColor = eyeRenderer.material.color;
        }
    }

    void Update()
    {
        if (battery == null || eyeRenderer == null) return;   // güvenlik

        // 1) Batarya bitti: sürekli kırmızı
        if (battery.IsEmpty())
        {
            eyeRenderer.material.color = warningColor;
            return;
        }

        // 2) Batarya yeterli: normal renk, sayaç sıfırla
        if (battery.GetPercent() >= warningLevel)
        {
            eyeRenderer.material.color = normalColor;
            timer = 0f;
            redOn = false;
            return;
        }

        // 3) Batarya az: yanıp sön
        timer += Time.deltaTime;
        if (timer >= blinkInterval)
        {
            timer = 0f;
            redOn = !redOn;   // kırmızıysa normal yap, normalse kırmızı yap
        }

        if (redOn)
        {
            eyeRenderer.material.color = warningColor;
        }
        else
        {
            eyeRenderer.material.color = normalColor;
        }
    }
}
