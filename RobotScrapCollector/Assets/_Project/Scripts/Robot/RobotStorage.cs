using UnityEngine;

// SAHİBİ: Furkan. Bu dosyanın içini sadece Furkan doldurur.
// İSKELET: Fonksiyon adlarını DEĞİŞTİRMEYİN, başkaları bunları kullanıyor.
public class RobotStorage : MonoBehaviour
{
    // Depoda yer var mı? (Çöp bunu sorup ona göre toplanır)
    public bool CanCollect()
    {
        return true; // geçici: Furkan dolduracak
    }

    // Depoya bir çöp ekle (değeri ve ağırlığı ile)
    public void AddTrash(int value, float weight)
    {
        // Furkan dolduracak
    }

    // Depoyu boşalt, değeri Scrap'e çevir (teslim noktası çağırır)
    public void Unload()
    {
        // Furkan dolduracak
    }

    // Toplam Scrap (ileride market kullanacak)
    public int GetScrap()
    {
        return 0; // geçici: Furkan dolduracak
    }
}