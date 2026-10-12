using UnityEngine;

// SAHİBİ: Ali
public class RobotBattery : MonoBehaviour
{
    [SerializeField] private float maxBattery = 100f;   // tam dolu batarya (market ile artacak)

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

    // Bataryanın doluluk oranı, 0 ile 1 arası (ileride ekrandaki batarya çubuğu kullanacak)
    public float GetPercent()
    {
        return currentBattery / maxBattery;
    }
}