using UnityEngine;

// SAHİBİ: Nazife — market geliştirmelerinin kuralları
public class UpgradeShop : MonoBehaviour
{
    [Header("Slot (fiyat: 2., 3., 4., 5. slot)")]
    [SerializeField] private int[] slotCosts = { 10, 25, 60, 140 };

    [Header("Batarya")]
    [SerializeField] private int[] batteryCosts = { 15, 35, 80 };
    [SerializeField] private float[] batteryIncreases = { 50f, 50f, 100f };   // 100 → 150 → 200 → 300

    [Header("Hız")]
    [SerializeField] private int[] speedCosts = { 15, 35, 80 };
    [SerializeField] private float[] speedIncreases = { 0.5f, 0.5f, 1f };     // 3 → 3.5 → 4 → 5

    private int batteryLevel;   // kaç batarya geliştirmesi alındı
    private int speedLevel;     // kaç hız geliştirmesi alındı

    private RobotStorage storage;   // Furkan: Scrap ve slotlar
    private RobotBattery battery;   // Ali: batarya
    private RobotSpeed speed;       // senin: hız

    void Awake()
    {
        storage = GetComponent<RobotStorage>();
        battery = GetComponent<RobotBattery>();
        speed = GetComponent<RobotSpeed>();
    }

    // ===== SLOT =====

    // Bir sonraki slotun fiyatı. -1 = bütün slotlar zaten açık.
    public int GetSlotCost()
    {
        int unlocked = storage.GetUnlockedSlots();
        if (unlocked >= storage.GetMaxSlots()) return -1;   // hepsi açık
        int index = unlocked - 1;                           // 1 slot açıksa ilk fiyat (index 0)
        if (index >= slotCosts.Length) return -1;           // tabloda fiyat kalmadı
        return slotCosts[index];
    }

    public bool BuySlot()
    {
        int cost = GetSlotCost();
        if (cost < 0) return false;                  // maksimum seviyede
        if (!storage.SpendScrap(cost)) return false; // para yetmiyor

        storage.UnlockSlot();
        Debug.Log("Bought slot for " + cost + " · slots: " + storage.GetUnlockedSlots());
        return true;
    }

    // ===== BATARYA =====

    public int GetBatteryCost()
    {
        if (batteryLevel >= batteryCosts.Length) return -1;   // maksimum
        return batteryCosts[batteryLevel];
    }

    public bool BuyBattery()
    {
        int cost = GetBatteryCost();
        if (cost < 0) return false;
        if (!storage.SpendScrap(cost)) return false;

        battery.IncreaseMax(batteryIncreases[batteryLevel]);
        batteryLevel++;
        Debug.Log("Bought battery for " + cost + " · level " + batteryLevel);
        return true;
    }

    // ===== HIZ =====

    public int GetSpeedCost()
    {
        if (speedLevel >= speedCosts.Length) return -1;
        return speedCosts[speedLevel];
    }

    public bool BuySpeed()
    {
        int cost = GetSpeedCost();
        if (cost < 0) return false;
        if (!storage.SpendScrap(cost)) return false;

        speed.IncreaseBaseSpeed(speedIncreases[speedLevel]);
        speedLevel++;
        Debug.Log("Bought speed for " + cost + " · level " + speedLevel);
        return true;
    }

    // ===== SEVİYE BİLGİSİ (Esra'nın paneli "Seviye 2/3" yazmak için kullanabilir) =====

    public int GetSlotLevel() { return storage.GetUnlockedSlots() - 1; }
    public int GetSlotMaxLevel() { return storage.GetMaxSlots() - 1; }
    public int GetBatteryLevel() { return batteryLevel; }
    public int GetBatteryMaxLevel() { return batteryCosts.Length; }
    public int GetSpeedLevel() { return speedLevel; }
    public int GetSpeedMaxLevel() { return speedCosts.Length; }

    
        // ===== AŞAMA 3 SÖZLEŞMESİ (Nazife dolduracak) =====

    public bool BuyMagnet() { return false; }
    public bool BuyBatPower() { return false; }
    public int GetMagnetCost() { return 0; }
    public int GetBatPowerCost() { return 0; }
}
