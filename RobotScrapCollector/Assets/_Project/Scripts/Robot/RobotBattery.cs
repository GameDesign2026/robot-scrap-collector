using UnityEngine;

// SAHİBİ: Ali
public class RobotBattery : MonoBehaviour
{
    [SerializeField] private float maxBattery = 100f;      // tam dolu batarya (market ile artar)
    [SerializeField] private float chargePerSecond = 25f;  // markette saniyede ne kadar dolar

    private float currentBattery;
    private bool emptyMessageShown;
    private bool charging;   // şu an markette mi (şarjda mı)?

    void Awake()
    {
        currentBattery = maxBattery;
    }

    void Update()
    {
        // Şarjda değilsek ya da zaten doluysa yapacak bir şey yok
        if (!charging) return;
        if (currentBattery >= maxBattery) return;

        // Her saniye chargePerSecond kadar doldur, maksimumu geçme
        currentBattery += chargePerSecond * Time.deltaTime;
        currentBattery = Mathf.Min(currentBattery, maxBattery);

        // Bir daha biterse "Battery empty!" mesajı tekrar yazılsın
        emptyMessageShown = false;
    }

    public void Drain(float amount)
    {
        if (charging) return;   // şarjdayken harcama yok (ikinci güvenlik)
        if (amount <= 0f) return;
        if (IsEmpty()) return;

        currentBattery -= amount;
        if (currentBattery < 0f) currentBattery = 0f;

        if (IsEmpty() && !emptyMessageShown)
        {
            Debug.Log("Battery empty!");
            emptyMessageShown = true;
        }
    }

    public void RechargeFull()
    {
        currentBattery = maxBattery;
        emptyMessageShown = false;
        Debug.Log("Battery full: " + currentBattery);
    }

    public bool IsEmpty()
    {
        return currentBattery <= 0f;
    }

    public float GetPercent()
    {
        return currentBattery / maxBattery;
    }

    public void IncreaseMax(float amount)
    {
        if (amount <= 0f) return;
        maxBattery += amount;
        Debug.Log("Max battery: " + maxBattery);
    }

    public float GetCurrent() { return currentBattery; }
    public float GetMax() { return maxBattery; }

    // ===== AŞAMA 1 =====

    // Markete girince true, çıkınca false (Esra'nın MarketZone'u çağırır)
    public void SetCharging(bool value)
    {
        charging = value;
        if (value) Debug.Log("Charging started");
        else Debug.Log("Charging stopped: " + Mathf.RoundToInt(currentBattery) + " / " + Mathf.RoundToInt(maxBattery));
    }

    public bool IsCharging()
    {
        return charging;
    }
}