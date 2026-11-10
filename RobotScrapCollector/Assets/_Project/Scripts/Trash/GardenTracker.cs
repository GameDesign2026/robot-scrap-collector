using UnityEngine;

// SAHİBİ: Sümeyra. İSKELET: fonksiyon adlarını değiştirmeyin.
// Bahçede (yerde) kaç çöp var?
public class GardenTracker : MonoBehaviour
{
    // Bir çöp bahçeye düştü
    public void Register(TrashItem item) { }

    // Bir çöp bahçeden gitti (alındı, teslim edildi, dışarı vuruldu)
    public void Unregister(TrashItem item) { }

    // Bahçede şu an kaç çöp var? (HUD ve gün sonu kontrolü)
    public int GetCount() { return 0; }

    // Bu nokta bahçenin (çitin) içinde mi?
    public bool IsInsideGarden(Vector3 point) { return true; }
}