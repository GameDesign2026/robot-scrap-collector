using UnityEngine;

// SAHİBİ: Ali. İSKELET: fonksiyon adlarını değiştirmeyin.
// Oyunun saati: 08:00'den 20:00'e
public class DayClock : MonoBehaviour
{
    // Şu anki saat, ondalıklı (örneğin 17.5 = 17:30)
    public float GetHour() { return 8f; }

    // Ekranda gösterilecek saat ("17:30")
    public string GetTimeText() { return "08:00"; }

    // Yayalar çöp atabilir mi? (19:00'dan sonra false)
    public bool CanThrow() { return true; }

    // 19:00–20:00 temizlik saati mi?
    public bool IsCleanupTime() { return false; }

    // Gün bitti mi? (20:00)
    public bool IsDayOver() { return false; }

    // Kaçıncı gün (1'den başlar)
    public int GetDay() { return 1; }

    // Yeni günü başlat (Esra'nın "Sonraki Gün" butonu çağırır)
    public void StartNewDay() { }
}