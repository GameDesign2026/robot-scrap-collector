using UnityEngine;

// SAHİBİ: Nazife. Bu dosyanın içini sadece Nazife doldurur.
// İSKELET: Fonksiyon adlarını DEĞİŞTİRMEYİN, başkaları bunları kullanıyor.
public class RobotSpeed : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3f;   // normal hız (tasarım: 3, market ile artacak)

    private float surfaceMultiplier = 1f;            // zemin çarpanı: normal 1, çamur 0.5, yağ 1.5

    // Zemin hız çarpanını ayarla (çamur 0.5, yağ 1.5)
    public void SetSurface(float multiplier)
    {
        if (multiplier <= 0f) return;   // 0 ya da eksi çarpan robotu durdurur, kabul etme
        surfaceMultiplier = multiplier;
    }

    // Normal zemine dön (çarpan 1)
    public void ResetSurface()
    {
        surfaceMultiplier = 1f;
    }

    // Şu anki hız çarpanı (ileride hareket kodu kullanacak)
    public float GetMultiplier()
    {
        return surfaceMultiplier;
    }

    // Robotun şu anki gerçek hızı (4. haftada hareket kodu kullanacak)
    public float GetSpeed()
    {
        return baseSpeed * surfaceMultiplier;
    }
}