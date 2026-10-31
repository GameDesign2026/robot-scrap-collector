using UnityEngine;

// SAHİBİ: Ali
public class RobotBattery : MonoBehaviour
{
    [SerializeField] private float maxBattery = 100f;   // tam dolu batarya (market ile artar)

    private float currentBattery;      // şu anki batarya
    private bool emptyMessageShown;    // "bitti" mesajı yazıldı mı?

    void Awake()
    {
        // Oyun başlarken batarya tam dolu
        currentBattery = maxBattery;
    }

    // Bataryayı belli bir miktar azalt
    public void Drain(float amount)
    {
        if (amount <= 0f) return;           // kontrol: eksi ya da sıfır miktar anlamsız
        if (IsEmpty()) return;              // kontrol: zaten bittiyse bir şey yapma

        currentBattery -= amount;

        if (currentBattery < 0f)            // sıfırın altına inmesin
        {
            currentBattery = 0f;
        }

        if (IsEmpty() && !emptyMessageShown) // bittiyse mesajı BİR KERE yaz
        {
            Debug.Log("Battery empty!");
            emptyMessageShown = true;
        }
    }

    // Bataryayı tamamen doldur
    public void RechargeFull()
    {
        currentBattery = maxBattery;
        emptyMessageShown = false;          // bir dahaki bitişte mesaj tekrar yazılabilsin
        Debug.Log("Battery full: " + currentBattery);
    }

    // Batarya bitti mi?
    public bool IsEmpty()
    {
        return currentBattery <= 0f;
    }

    // Bataryanın doluluk oranı, 0 ile 1 arası (ekrandaki batarya çubuğu kullanır)
    public float GetPercent()
    {
        return currentBattery / maxBattery;
    }

    // ===== 5. HAFTA =====

    // Maksimum bataryayı artır (Nazife'nin marketi çağırır)
    public void IncreaseMax(float amount)
    {
        if (amount <= 0f) return;   // eksi ya da sıfır artış anlamsız
        maxBattery += amount;       // şu anki batarya artmaz, istasyonda doldurulur
        Debug.Log("Max battery: " + maxBattery);
    }

    // Şu anki batarya (HUD yazısı için)
    public float GetCurrent()
    {
        return currentBattery;
    }

    // Maksimum batarya (HUD yazısı için)
    public float GetMax()
    {
        return maxBattery;
    }
}