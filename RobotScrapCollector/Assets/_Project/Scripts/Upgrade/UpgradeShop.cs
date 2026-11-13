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

    
    // ===== AŞAMA 3 (Nazife) =====

    [Header("Mıknatıs (E ile alma mesafesi)")]
    [SerializeField] private int[] magnetCosts = { 20, 45 };
    [SerializeField] private float[] magnetRanges = { 3.5f, 4.5f };

    [Header("Güçlü Sopa (vuruş alanı)")]
    [SerializeField] private int[] batPowerCosts = { 25, 60 };
    [SerializeField] private float[] batRadii = { 1.4f, 1.8f };

    private int magnetLevel;     // kaç kere alındı
    private int batPowerLevel;

    // Esra'nın market paneli sorar: bir sonraki seviyenin fiyatı (-1 = maks)
    public int GetMagnetCost()
    {
        if (magnetLevel >= magnetCosts.Length) return -1;
        return magnetCosts[magnetLevel];
    }

    public int GetBatPowerCost()
    {
        if (batPowerLevel >= batPowerCosts.Length) return -1;
        return batPowerCosts[batPowerLevel];
    }

    // Esra'nın butonu çağırır
    public bool BuyMagnet()
    {
        int cost = GetMagnetCost();
        if (cost < 0) return false;                               // zaten maks

        RobotStorage store = GetComponent<RobotStorage>();
        if (store == null || !store.SpendScrap(cost)) return false;   // para yetmedi

        // Yeni mesafeyi Sümeyra'nın etkileşim kodunda ayarla
        RobotInteractor interactor = GetComponent<RobotInteractor>();
        if (interactor != null && magnetLevel < magnetRanges.Length)
        {
            interactor.SetRange(magnetRanges[magnetLevel]);
        }

        magnetLevel++;
        Debug.Log("Mıknatıs seviye " + magnetLevel);
        return true;
    }

    public bool BuyBatPower()
    {
        int cost = GetBatPowerCost();
        if (cost < 0) return false;

        RobotStorage store = GetComponent<RobotStorage>();
        if (store == null || !store.SpendScrap(cost)) return false;

        // Yeni yarıçapı Sümeyra'nın sopasında ayarla
        BatController bat = GetComponent<BatController>();
        if (bat != null && batPowerLevel < batRadii.Length)
        {
            bat.SetHitRadius(batRadii[batPowerLevel]);
        }

        batPowerLevel++;
        Debug.Log("Güçlü sopa seviye " + batPowerLevel);
        return true;
    }
}
