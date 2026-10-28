using UnityEngine;

// SAHİBİ: Furkan
public class RobotStorage : MonoBehaviour
{
    [SerializeField] private int capacity = 1;   // aynı anda kaç çöp taşıyabilir (market ile artacak)

    private int itemCount;        // şu an depoda kaç çöp var
    private int carriedValue;     // depodaki çöplerin toplam değeri
    private float carriedWeight;  // depodaki çöplerin toplam ağırlığı
    private int scrap;            // teslim edilip kazanılan toplam Scrap

    // Depoda yer var mı?
    public bool CanCollect()
    {
        return itemCount < capacity;
    }

    // Depoya bir çöp ekle
    public void AddTrash(int value, float weight)
    {
        if (!CanCollect()) return;   // kontrol: yer yoksa hiçbir şey yapma

        itemCount++;
        carriedValue += value;
        carriedWeight += weight;

        Debug.Log("Storage " + itemCount + "/" + capacity
                  + " · value " + carriedValue
                  + " · weight " + carriedWeight);
    }

    // Depoyu boşalt, taşınan değeri Scrap'e çevir
    public void Unload()
    {
        if (itemCount == 0) return;  // kontrol: depo boşsa hiçbir şey yapma

        scrap += carriedValue;
        Debug.Log("Delivered " + carriedValue + " · Scrap: " + scrap);

        itemCount = 0;
        carriedValue = 0;
        carriedWeight = 0f;
    }

    // Toplam Scrap
    public int GetScrap()
    {
        return scrap;
    }

    // Taşınan toplam ağırlık (4. haftada robot ağırken yavaşlayacak)
    public float GetCarriedWeight()
    {
        return carriedWeight;

    }
    public int GetItemCount()
    {
        return itemCount;
    }

    // Kapasite kaç (ileride ekrandaki "Storage 1/1" yazısı kullanacak)
    public int GetCapacity()
    {
        return capacity;
    }
        // ===== 5. HAFTA SÖZLEŞMESİ (Furkan dolduracak) =====

    // Çöpü boş bir slota koymaya çalış. Koyabildiyse true.
    public bool TryAddItem(TrashItem item) { return false; }

    // Seçili slottaki çöpü yere bırak (Q)
    public void DropSelected() { }

    // Seçili slotun numarası (0'dan başlar)
    public int GetSelectedIndex() { return 0; }

    // Kaç slot açık (başta 1)
    public int GetUnlockedSlots() { return 1; }

    // Toplam slot sayısı (kilitliler dahil)
    public int GetMaxSlots() { return 5; }

    // Bu slotta hangi çöp var? (boşsa null)
    public TrashItem GetItemInSlot(int index) { return null; }

    // Bir slot daha aç (market)
    public void UnlockSlot() { }

    // Scrap harca. Yeterliyse düşer ve true döner.
    public bool SpendScrap(int amount) { return false; }


}